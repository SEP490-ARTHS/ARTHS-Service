using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface ICartService
    {
        Task<CartViewModel> GetCartByCustomerId(Guid customerId);
        Task<CartViewModel> AddToCart(Guid customerId, List<CreateCartModel> list);
        Task<CartViewModel> UpdateCart(Guid cartId, UpdateCartModel model);
    }
}
