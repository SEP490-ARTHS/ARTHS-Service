using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
