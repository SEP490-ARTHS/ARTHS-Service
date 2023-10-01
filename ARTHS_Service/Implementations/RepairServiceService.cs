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
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class RepairServiceService : BaseService, IRepairServiceService
    {
        private readonly IRepairServiceRepository _repairRepository;

        private readonly ICloudStorageService _cloudStorageService;

        public RepairServiceService(IUnitOfWork unitOfWork, IMapper mapper, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _repairRepository = unitOfWork.RepairService;

            _cloudStorageService = cloudStorageService;
        }


        public async Task<List<RepairServiceViewModel>> GetRepairServices(RepairServiceFilterModel filter)
        {
            var query = _repairRepository.GetAll();
            
            if(filter.Name != null)
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
            var result = 0;
            var repairServiceId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    repairServiceId = Guid.NewGuid();
                    var imageUrl = await _cloudStorageService.Upload(repairServiceId, model.Image.ContentType, model.Image.OpenReadStream());
                    var repairService = new RepairService
                    {
                        Id = repairServiceId,
                        Name = model.Name,
                        Price = model.Price,
                        ImageUrl = imageUrl,
                        Description = model.Description,
                        Status = RepairServiceStatus.Active
                    };

                    _repairRepository.Add(repairService);
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
                throw new NotFoundException("Không tìm thấy repair service");
            }

            repairService.Name = model.Name ?? repairService.Name;
            repairService.Price = model.Price ?? repairService.Price;
            repairService.Description = model.Description ?? repairService.Description;
            repairService.Status = model.Status ?? repairService.Status;

            if (model.Image != null)
            {
                //xóa hình cũ và update hình mới trong firebase
                await _cloudStorageService.Delete(id);
                var newImageUrl = await _cloudStorageService.Upload(id, model.Image.ContentType, model.Image.OpenReadStream());
                repairService.ImageUrl = newImageUrl;
            }

            _repairRepository.Update(repairService);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetRepairService(id) : null!;
        }
    }
}
