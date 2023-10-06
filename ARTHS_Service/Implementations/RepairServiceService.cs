using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Filters;
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

        private readonly ICloudStorageService _cloudStorageService;

        public RepairServiceService(IUnitOfWork unitOfWork, IMapper mapper, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _repairRepository = unitOfWork.RepairService;
            _imageRepository = unitOfWork.Image;

            _cloudStorageService = cloudStorageService;
        }


        public async Task<List<RepairServiceViewModel>> GetRepairServices(RepairServiceFilterModel filter)
        {
            var query = _repairRepository.GetAll();

            if (filter.Name != null)
            {
                query = query.Where(repair => repair.Name.Contains(filter.Name));
            }

            
            return await query
                .ProjectTo<RepairServiceViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
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
            var repairServiceId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    repairServiceId = Guid.NewGuid();
                    var repairService = new RepairService
                    {
                        Id = repairServiceId,
                        Name = model.Name,
                        Price = model.Price,
                        Description = model.Description,
                        Status = RepairServiceStatus.Active
                    };

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

            if (model.Images != null && model.Images.Count > 0)
            {
                if(model.Images.Count > 4)
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

                await UpdateRepairServiceImage(id, model.Images);
            }

            repairService.Name = model.Name ?? repairService.Name;
            repairService.Price = model.Price ?? repairService.Price;
            repairService.Description = model.Description ?? repairService.Description;
            repairService.Status = model.Status ?? repairService.Status;

            

            _repairRepository.Update(repairService);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetRepairService(id) : null!;
        }





        //PRIVATE METHOD
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

        private async Task<bool> UpdateRepairServiceImage(Guid id, ICollection<IFormFile> images)
        {
            var existImages = await _imageRepository.GetMany(image => image.RepairServiceId.Equals(id)).ToListAsync();
            foreach (var image in existImages)
            {
                await _cloudStorageService.Delete(image.Id);
                //_imageRepository.Remove(image);
            }
            _imageRepository.RemoveRange(existImages);
            //await _unitOfWork.SaveChanges();

            var uploadedImages = await CreateRepairServiceImage(id, images);
            return uploadedImages != null && uploadedImages.Count == images.Count;
        }
    }
}
