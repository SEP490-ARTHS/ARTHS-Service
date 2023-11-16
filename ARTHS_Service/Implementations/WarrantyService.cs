using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class WarrantyService : BaseService, IWarrantyService
    {
        private readonly IWarrantyRepository _WarrantyRepository;
        public WarrantyService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _WarrantyRepository = unitOfWork.Warranty;
        }

        public async Task<List<WarrantyViewModel>> GetWarranties(WarrantyFilterModel filter)
        {
            var query = _WarrantyRepository.GetAll();
            if (filter.Duration > 0)
            {
                query = query.Where(warranty => warranty.Duration == filter.Duration);
            }

            return await query
                .OrderBy(warranty => warranty.Duration)
                .ProjectTo<WarrantyViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<WarrantyViewModel> GetWarranty(Guid id)
        {
            return await _WarrantyRepository.GetMany(w => w.Id.Equals(id))
                .ProjectTo<WarrantyViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy"); ;
        }

        public async Task<WarrantyViewModel> CreateWarranty(CreateWarrantyRequest model)
        {
            var result = 0;
            var warrantyId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    warrantyId = Guid.NewGuid();
                    var warranty = new Warranty
                    {
                        Id = warrantyId,
                        Duration = model.Duration,
                        Description = model.Description,
                    };

                    _WarrantyRepository.Add(warranty);
                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }

                return result > 0 ? await GetWarranty(warrantyId) : null!;
            }
        }

        public async Task<WarrantyViewModel> UpdateWarranty(Guid id, UpdateWarrantyRequest model)
        {
            var warranty = await _WarrantyRepository.GetMany(w => w.Id.Equals(id)).FirstOrDefaultAsync();

            if (warranty == null)
            {
                throw new NotFoundException("Không tìm thấy");
            }

            warranty.Duration = model.Duration ?? warranty.Duration;
            warranty.Description = model.Description ?? warranty.Description;

            _WarrantyRepository.Update(warranty);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetWarranty(id) : null!;
        }

        public async Task<WarrantyViewModel> DeleteWarranty(Guid Id)
        {
            var Warranty = await _WarrantyRepository.GetMany(Warranty => Warranty.Id.Equals(Id)).FirstOrDefaultAsync();
            if (Warranty != null)
            {
                _WarrantyRepository.Remove(Warranty);

                var result = await _unitOfWork.SaveChanges();
                if (result > 0)
                {
                    return new WarrantyViewModel { };
                }
                throw new Exception("xóa không thành công");
            }
            throw new NotFoundException("không tìm thấy");
        }
    }
}
