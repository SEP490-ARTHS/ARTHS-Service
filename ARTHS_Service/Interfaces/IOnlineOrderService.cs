using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IOnlineOrderService
    {
        Task<List<OnlineOrderViewModel>> GetOrders();
        Task<OnlineOrderViewModel> GetOrder(Guid id);
        Task<List<OnlineOrderViewModel>> GetOrdersByCustomerId(Guid customerId);
        Task<OnlineOrderViewModel> CreateOrder(Guid cartId, CreateOnlineOrderModel model);
        Task<OnlineOrderViewModel> UpdateOrder(Guid id, UpdateOnlineOrderModel model);

    }
}
