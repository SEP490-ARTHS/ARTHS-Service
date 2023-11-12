using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IOrderService
    {
        Task<ListViewModel<OrderViewModel>> GetOrders(OrderFilterModel filter, PaginationRequestModel pagination);
        Task<OrderViewModel> GetOrder(string Id);
        Task<OrderViewModel> CreateOrderOnline(Guid customerId, CreateOrderOnlineModel model);
        Task<OrderViewModel> UpdateOrderOnline(string Id, UpdateOrderOnlineModel model);


        Task<OrderViewModel> CreateOrderOffline(Guid tellerId, CreateOrderOfflineModel model);
        Task<OrderViewModel> UpdateOrderOffline(string Id, UpdateInStoreOrderModel model);
    }
}
