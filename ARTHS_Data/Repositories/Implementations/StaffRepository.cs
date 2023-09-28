using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class StaffRepository : Repository<StaffAccount>, IStaffRepository
    {
        public StaffRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
