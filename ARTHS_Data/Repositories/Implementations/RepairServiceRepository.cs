using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class RepairServiceRepository : Repository<RepairService>, IRepairServiceRepository
    {
        public RepairServiceRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
