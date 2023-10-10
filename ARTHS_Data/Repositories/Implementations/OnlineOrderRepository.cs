using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class OnlineOrderRepository : Repository<OnlineOrder>, IOnlineOrderRepository
    {
        public OnlineOrderRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
