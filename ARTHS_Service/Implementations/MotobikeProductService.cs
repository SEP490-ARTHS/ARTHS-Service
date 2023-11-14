using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class MotobikeProductService : BaseService, IMotobikeProductService
    {
        private readonly IMotobikeProductRepository _motobikeProductRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IMotobikeProductPriceRepository _motobikeProductPriceRepository;
        private readonly IVehicleRepository _vehicleRepository;

        private readonly ICloudStorageService _cloudStorageService;

        public MotobikeProductService(IUnitOfWork unitOfWork, IMapper mapper, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _motobikeProductRepository = _unitOfWork.MotobikeProduct;
            _imageRepository = _unitOfWork.Image;
            _motobikeProductPriceRepository = _unitOfWork.MotobikeProductPrice;
            _vehicleRepository = _unitOfWork.Vehicle;

            _cloudStorageService = cloudStorageService;
        }

        public async Task<ListViewModel<MotobikeProductDetailViewModel>> GetMotobikeProducts(MotobikeProductFilterModel filter, PaginationRequestModel pagination)
        {
            var baseQuery = _motobikeProductRepository.GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                baseQuery = baseQuery.Where(product => product.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrEmpty(filter.Category))
            {
                baseQuery = baseQuery.Where(product => product.Category != null && product.Category.CategoryName.Contains(filter.Category));
            }

            if (!string.IsNullOrEmpty(filter.VehiclesName))
            {
                baseQuery = baseQuery.Where(product => product.Vehicles.Any(vehicle => vehicle.VehicleName.Contains(filter.VehiclesName)));
            }

            if (filter.DiscountId.HasValue)
            {
                baseQuery = baseQuery.Where(product => product.DiscountId.Equals(filter.DiscountId.Value));
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                baseQuery = baseQuery.Where(product => product.Status.Contains(filter.Status));
            }

            // Sorting logic
            if (filter.SortByNameAsc.HasValue)
            {
                baseQuery = filter.SortByNameAsc.Value
                    ? baseQuery.OrderBy(p => p.Name)
                    : baseQuery.OrderByDescending(p => p.Name);
            }

            if (filter.SortByPriceAsc.HasValue)
            {
                baseQuery = filter.SortByPriceAsc.Value
                    ? baseQuery.OrderBy(p => p.PriceCurrent)
                    : baseQuery.OrderByDescending(p => p.PriceCurrent);
            }
            if (!filter.SortByNameAsc.HasValue && !filter.SortByPriceAsc.HasValue)
            {
                baseQuery = baseQuery.OrderByDescending(p => p.CreateAt);
            }

            var totalRow = await baseQuery.AsNoTracking().CountAsync();
            var paginatedQuery = baseQuery
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);

            var products = await paginatedQuery
                .ProjectTo<MotobikeProductDetailViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            return new ListViewModel<MotobikeProductDetailViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = products
            };
        }

        public async Task<MotobikeProductDetailViewModel> GetMotobikeProduct(Guid id)
        {
            return await _motobikeProductRepository.GetMany(product => product.Id.Equals(id))
                .ProjectTo<MotobikeProductDetailViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy sản phẩm.");
        }

        public async Task<MotobikeProductDetailViewModel> CreateMotobikeProduct(CreateMotobikeProductModel model)
        {
            var imageCount = model.Images.Count();
            if (imageCount < 1 || imageCount > 4)
            {
                throw new BadRequestException("Phải có ít nhất một hình để tạo và không được quá 4 hình.");
            }
            foreach (IFormFile image in model.Images)
            {
                if (!image.ContentType.StartsWith("image/"))
                {
                    throw new BadRequestException("File không phải là hình ảnh");
                }
            }

            var result = 0;
            var motobikeProductId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    var vehicles = await _vehicleRepository.GetMany(vehicle => model.VehiclesId.Contains(vehicle.Id)).ToListAsync();
                    if (vehicles.Count == 0)
                    {
                        throw new BadRequestException("Vui lòng nhập lại vehicle id.");
                    }

                    motobikeProductId = Guid.NewGuid();
                    var motobikeProduct = new MotobikeProduct
                    {
                        Id = motobikeProductId,
                        DiscountId = model.DiscountId,
                        WarrantyId = model.WarrantyId,
                        CategoryId = model.CategoryId,
                        Name = model.Name,
                        PriceCurrent = model.PriceCurrent,
                        InstallationFee = model.InstallationFee,
                        Quantity = model.Quantity,
                        Description = model.Description,
                        Vehicles = vehicles,
                        Status = ProductStatus.Active,
                    };
                    _motobikeProductRepository.Add(motobikeProduct);
                    CreateMotobikeProductPrice(motobikeProductId, model.PriceCurrent);
                    await CreateMotobikeProductImage(motobikeProductId, model.Images);
                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetMotobikeProduct(motobikeProductId) : null!;
        }


        public async Task<MotobikeProductDetailViewModel> UpdateMotobikeProduct(Guid id, UpdateMotobikeProductModel model)
        {
            var motobikeProduct = await _motobikeProductRepository.GetMany(product => product.Id.Equals(id)).Include(product => product.Vehicles).FirstOrDefaultAsync();
            if (motobikeProduct == null)
            {
                throw new NotFoundException("Không tìm thấy product.");
            }

            //if (model.Images != null && model.Images.Count > 0)
            //{
            //    if (model.Images.Count > 4)
            //    {
            //        throw new BadRequestException("Chỉ được chứa bốn hình ảnh.");
            //    }

            //    foreach (IFormFile image in model.Images)
            //    {
            //        if (!image.ContentType.StartsWith("image/"))
            //        {
            //            throw new BadRequestException("File không phải là hình ảnh");
            //        }
            //    }

            //    await UpdateMotobikeProductImage(id, model.Images);
            //}

            motobikeProduct.PriceCurrent = model.PriceCurrent ?? motobikeProduct.PriceCurrent;

            if (model.PriceCurrent != null)
            {
                CreateMotobikeProductPrice(id, (int)model.PriceCurrent);
            }

            motobikeProduct.Name = model.Name ?? motobikeProduct.Name;
            motobikeProduct.Quantity = model.Quantity ?? motobikeProduct.Quantity;
            motobikeProduct.Description = model.Description ?? motobikeProduct.Description;
            motobikeProduct.Status = model.Status ?? motobikeProduct.Status;
            motobikeProduct.DiscountId = model.DiscountId ?? motobikeProduct.DiscountId;
            motobikeProduct.WarrantyId = model.WarrantyId ?? motobikeProduct.WarrantyId;
            motobikeProduct.CategoryId = model.CategoryId ?? motobikeProduct.CategoryId;
            motobikeProduct.InstallationFee = model.InstallationFee ?? motobikeProduct.InstallationFee;
            motobikeProduct.UpdateAt = DateTime.Now;

            if (model.VehiclesId != null && model.VehiclesId.Count > 0)
            {
                var vehicles = await _vehicleRepository.GetMany(vehicle => model.VehiclesId.Contains(vehicle.Id)).ToListAsync();
                if (vehicles.Count == 0)
                {
                    throw new BadRequestException("Vui lòng nhập lại vehicle id.");
                }

                motobikeProduct.Vehicles.Clear();
                Console.Write(motobikeProduct.Vehicles);
                motobikeProduct.Vehicles = vehicles;
            }

            _motobikeProductRepository.Update(motobikeProduct);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetMotobikeProduct(id) : null!;
        }

        //PRIVATE METHOD
        private bool CreateMotobikeProductPrice(Guid id, int price)
        {
            var motobikeProductPrice = new MotobikeProductPrice
            {
                Id = Guid.NewGuid(),
                MotobikeProductId = id,
                DateApply = DateTime.UtcNow,
                PriceCurrent = price
            };
            _motobikeProductPriceRepository.Add(motobikeProductPrice);
            return true;
        }

        private async Task<ICollection<Image>> CreateMotobikeProductImage(Guid id, ICollection<IFormFile> images)
        {
            var listImage = new List<Image>();
            foreach (IFormFile image in images)
            {
                var imageId = Guid.NewGuid();
                var url = await _cloudStorageService.Upload(imageId, image.ContentType, image.OpenReadStream());
                var newImage = new Image
                {
                    Id = imageId,
                    MotobikeProductId = id,
                    ImageUrl = url
                };
                listImage.Add(newImage);
            }
            _imageRepository.AddRange(listImage);
            return listImage;
        }

        public async Task<MotobikeProductDetailViewModel> UpdateMotobikeProductImage(Guid motobikeProductId, UpdateImageModel model)
        {
            if (!model.Image.ContentType.StartsWith("image/"))
            {
                throw new BadRequestException("file không phải là hình ảnh");
            }
            var imageId = Guid.NewGuid();
            var url = await _cloudStorageService.Upload(imageId, model.Image.ContentType, model.Image.OpenReadStream());
            var newImage = new Image
            {
                Id = imageId,
                MotobikeProductId = motobikeProductId,
                ImageUrl = url
            };
            _imageRepository.Add(newImage);
            return await _unitOfWork.SaveChanges() > 0 ? await GetMotobikeProduct(motobikeProductId) : null!;
        }

        public async Task RemoveMotobikeProductImage(Guid imageId)
        {
            var existImage = await _imageRepository.GetMany(image => image.Id.Equals(imageId)).FirstOrDefaultAsync();
            if (existImage != null)
            {
                await _cloudStorageService.Delete(imageId);
                _imageRepository.Remove(existImage);

                await _unitOfWork.SaveChanges();
            }
            else
            {
                throw new NotFoundException("Không tìm thấy image");
            }
        }
    }
}
