using ARTHS_Data;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class StaffService : BaseService, IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        public StaffService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _staffRepository = unitOfWork.Staff;
        }

        public async Task<StaffViewModel> GetStaff(Guid id)
        {
            return await _staffRepository.GetMany(staff => staff.AccountId.Equals(id))
                .Include(staff => staff.Account)
                .ProjectTo<StaffViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? null!;
        }
    }
}
