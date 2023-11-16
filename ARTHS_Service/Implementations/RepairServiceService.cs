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
    public class RepairServiceService : BaseService, IRepairServiceService
    {
        private readonly IRepairServiceRepository _repairRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IMotobikeProductRepository _motobikeProductRepository;

        private readonly ICloudStorageService _cloudStorageService;

        public RepairServiceService(IUnitOfWork unitOfWork, IMapper mapper, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _repairRepository = unitOfWork.RepairService;
            _imageRepository = unitOfWork.Image;
            _motobikeProductRepository = unitOfWork.MotobikeProduct;

            _cloudStorageService = cloudStorageService;
        }


        public async Task<ListViewModel<RepairServiceViewModel>> GetRepairServices(RepairServiceFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _repairRepository.GetAll().AsQueryable();

            if (filter.Name != null)
            {
                query = query.Where(repair => repair.Name.Contains(filter.Name));
            }
            if (!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(repair => repair.Status.Contains(filter.Status));
            }
            // Sorting logic
            if (filter.SortByNameAsc.HasValue)
            {
                query = filter.SortByNameAsc.Value
                    ? query.OrderBy(p => p.Name)
                    : query.OrderByDescending(p => p.Name);
            }

            if (filter.SortByPriceAsc.HasValue)
            {
                query = filter.SortByPriceAsc.Value
                    ? query.OrderBy(p => p.Price)
                    : query.OrderByDescending(p => p.Price);
            }
            if (!filter.SortByNameAsc.HasValue && !filter.SortByPriceAsc.HasValue)
            {
                query = query.OrderByDescending(p => p.CreateAt);
            }

            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);

            var services = await paginatedQuery
               .ProjectTo<RepairServiceViewModel>(_mapper.ConfigurationProvider)
               .AsNoTracking()
               .ToListAsync();

            if (services != null || services != null && services.Any())
            {
                return new ListViewModel<RepairServiceViewModel>
                {
                    Pagination = new PaginationViewModel
                    {
                        PageNumber = pagination.PageNumber,
                        PageSize = pagination.PageSize,
                        TotalRow = totalRow
                    },
                    Data = services
                };
            }
            return null!;
        }

        public async Task<RepairServiceViewModel> GetRepairService(Guid id)
        {
            return await _repairRepository.GetMany(repair => repair.Id.Equals(id))
                .ProjectTo<RepairServiceViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy dịch vụ sữa chữa");
        }

        public async Task<RepairServiceViewModel> CreateRepairService(CreateRepairServiceModel model)
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
            var repairServiceId = Guid.NewGuid();
            var repairService = new RepairService
            {
                Id = repairServiceId,
                Name = model.Name,
                Price = model.Price,
                Duration = model.Duration,
                ReminderInterval = model.ReminderInterval,
                WarrantyDuration = model.WarrantyDuration,
                DiscountId = model.DiscountId,
                Description = model.Description,
                Status = RepairServiceStatus.Active,

            };

            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    _repairRepository.Add(repairService);
                    await CreateRepairServiceImage(repairServiceId, model.Images);
                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetRepairService(repairServiceId) : null!;
        }

        public async Task<RepairServiceViewModel> UpdateRepairService(Guid id, UpdateRepairServiceModel model)
        {
            var repairService = await _repairRepository.GetMany(repair => repair.Id.Equals(id)).FirstOrDefaultAsync();

            if (repairService == null)
            {
                throw new NotFoundException("Không tìm thấy repair service.");
            }

            

            repairService.Name = model.Name ?? repairService.Name;
            repairService.Price = model.Price ?? repairService.Price;
            repairService.Duration = model.Duration ?? repairService.Duration;
            repairService.WarrantyDuration = model.WarrantyDuration ?? repairService.WarrantyDuration;
            repairService.DiscountId = model.DiscountId ?? repairService.DiscountId;
            repairService.ReminderInterval = model.ReminderInterval ?? repairService.ReminderInterval;
            repairService.Description = model.Description ?? repairService.Description;
            repairService.Status = model.Status ?? repairService.Status;
            



            _repairRepository.Update(repairService);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetRepairService(id) : null!;
        }





        //PRIVATE METHOD

        //private async Task AddProductToRepairService(Guid repairServiceId, List<Guid> productIds)
        //{
        //    var listProduct = new List<MotobikeProduct>();
        //    foreach (var productId in productIds)
        //    {
        //        var product = await _motobikeProductRepository.GetMany(product => product.Id.Equals(productId)).FirstOrDefaultAsync();
        //        if (product == null) throw new NotFoundException($"Không tìm thấy product with id : {productId}");
        //        product.RepairServiceId = repairServiceId;
        //        listProduct.Add(product);
        //    }
        //    _motobikeProductRepository.UpdateRange(listProduct);
        //}

        //private async Task UpdateProductToRepairService(Guid repairServiceId, List<Guid> newProductIds)
        //{
        //    var listProduct = new List<MotobikeProduct>();
        //    var currentProductIds = await _motobikeProductRepository.GetMany(product => product.RepairServiceId.Equals(repairServiceId))
        //        .Select(product => product.Id)
        //        .ToListAsync();
        //    //có trong tập thứ nhất nhưng không có trong tập thứ 2
        //    var productIdsToAdd = newProductIds.Except(currentProductIds).ToList();
        //    var productIdsToRemove = currentProductIds.Except(newProductIds).ToList();

        //    foreach(var productId in productIdsToRemove)
        //    {
        //        var product = await _motobikeProductRepository.GetMany(product => product.Id.Equals(productId)).FirstOrDefaultAsync();
        //        product!.RepairServiceId = null;
        //        listProduct.Add(product);
        //    }
        //    foreach(var productId in productIdsToAdd)
        //    {
        //        var product = await _motobikeProductRepository.GetMany(product => product.Id.Equals(productId)).FirstOrDefaultAsync();
        //        if (product == null) throw new NotFoundException($"Không tìm thấy product id {productId}");
        //        product.RepairServiceId = repairServiceId;
        //        listProduct.Add(product);
        //    }
        //    _motobikeProductRepository.UpdateRange(listProduct);
        //}

        private async Task<ICollection<Image>> CreateRepairServiceImage(Guid id, ICollection<IFormFile> images)
        {
            var listImage = new List<Image>();
            foreach (IFormFile image in images)
            {
                var imageId = Guid.NewGuid();
                var url = await _cloudStorageService.Upload(imageId, image.ContentType, image.OpenReadStream());
                var newImage = new Image
                {
                    Id = imageId,
                    RepairServiceId = id,
                    ImageUrl = url
                };
                listImage.Add(newImage);
            }
            _imageRepository.AddRange(listImage);
            return listImage;
        }

        public async Task<RepairServiceViewModel> UpdateRepairServiceImage(Guid repairServiceId, UpdateImageModel model)
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
                RepairServiceId = repairServiceId,
                ImageUrl = url
            };
            _imageRepository.Add(newImage);
            return await _unitOfWork.SaveChanges() > 0 ? await GetRepairService(repairServiceId) : null!;
        }

        public async Task RemoveRepairServieImage(Guid imageId)
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
