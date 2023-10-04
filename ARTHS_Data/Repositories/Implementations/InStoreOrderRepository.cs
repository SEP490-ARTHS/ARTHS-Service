using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class InStoreOrderRepository : Repository<InStoreOrder>, IInStoreOrderRepository
    {
        public InStoreOrderRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
