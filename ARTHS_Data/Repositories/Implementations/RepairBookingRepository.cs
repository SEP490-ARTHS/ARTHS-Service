using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class RepairBookingRepository : Repository<RepairBooking>, IRepairBookingRepository
    {
        public RepairBookingRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
