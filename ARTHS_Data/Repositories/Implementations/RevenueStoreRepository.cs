using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class RevenueStoreRepository : Repository<RevenueStore>, IRevenueStoreRepository
    {
        public RevenueStoreRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
