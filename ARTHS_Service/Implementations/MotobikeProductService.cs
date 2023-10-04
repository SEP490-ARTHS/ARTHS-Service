using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Post;
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

        public async Task<List<MotobikeProductViewModel>> GetMotobikeProducts()
        {
            var query = _motobikeProductRepository.GetAll();

            return await query
                .ProjectTo<MotobikeProductViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
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
                    if(vehicles.Count == 0)
                    {
                        throw new BadRequestException("Vui lòng nhập lại vehicle id.");
                    }

                    motobikeProductId = Guid.NewGuid();
                    var motobikeProduct = new MotobikeProduct
                    {
                        Id = motobikeProductId,
                        RepairServiceId = model.RepairServiceId,
                        DiscountId = model.DiscountId,
                        WarrantyId = model.WarrantyId,
                        CategoryId = model.CategoryId,
                        Name = model.Name,
                        PriceCurrent = model.PriceCurrent,
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
                }catch (Exception)
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
            if(motobikeProduct == null)
            {
                throw new NotFoundException("Không tìm thấy product.");
            }

            if (model.Images != null && model.Images.Count > 0)
            {
                if (model.Images.Count > 4)
                {
                    throw new BadRequestException("Chỉ được chứa bốn hình ảnh.");
                }

                foreach (IFormFile image in model.Images)
                {
                    if (!image.ContentType.StartsWith("image/"))
                    {
                        throw new BadRequestException("File không phải là hình ảnh");
                    }
                }

                await UpdateMotobikeProductImage(id, model.Images);
            }
            motobikeProduct.PriceCurrent = model.PriceCurrent ?? motobikeProduct.PriceCurrent;

            if(model.PriceCurrent != null)
            {
                CreateMotobikeProductPrice(id, (int)model.PriceCurrent);
            }

            motobikeProduct.Name = model.Name ?? motobikeProduct.Name;
            motobikeProduct.Quantity = model.Quantity ?? motobikeProduct.Quantity;
            motobikeProduct.Description = model.Description ?? motobikeProduct.Description;
            motobikeProduct.Status = model.Status ?? motobikeProduct.Status;
            motobikeProduct.RepairServiceId = model.RepairServiceId ?? motobikeProduct.RepairServiceId;
            motobikeProduct.DiscountId = model.DiscountId ?? motobikeProduct.DiscountId;
            motobikeProduct.WarrantyId = model.WarrantyId ?? motobikeProduct.WarrantyId;
            motobikeProduct.CategoryId = model.CategoryId ?? motobikeProduct.CategoryId;
            motobikeProduct.UpdateAt = DateTime.Now;

            if(model.VehiclesId != null && model.VehiclesId.Count > 0)
            {
                var vehicles = await _vehicleRepository.GetMany(vehicle => model.VehiclesId.Contains(vehicle.Id)).ToListAsync();
                if(vehicles.Count == 0)
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
            foreach(IFormFile image in images)
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

        private async Task<bool> UpdateMotobikeProductImage(Guid id, ICollection<IFormFile> images)
        {
            var existImages = await _imageRepository.GetMany(image => image.MotobikeProductId.Equals(id)).ToListAsync();
            foreach (var image in existImages)
            {
                await _cloudStorageService.Delete(image.Id);
            }
            _imageRepository.RemoveRange(existImages);
            
            var uploadedImages = await CreateMotobikeProductImage(id, images);
            return uploadedImages != null && uploadedImages.Count == images.Count;
        }

    }
}
