using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
