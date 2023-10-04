using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class InStoreOrderDetailRepository : Repository<InStoreOrderDetail>, IInStoreOrderDetailRepository
    {
        public InStoreOrderDetailRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
