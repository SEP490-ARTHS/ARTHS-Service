using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IInStoreOrderService
    {
        Task<List<BasicInStoreOrderViewModel>> GetInStoreOrders(InStoreOrderFilterModel filter);
        Task<InStoreOrderViewModel> GetInStoreOrder(string id);
        Task<InStoreOrderViewModel> CreateInStoreOrder(Guid tellerId, CreateInStoreOrderModel model);
        Task<InStoreOrderViewModel> UpdateInStoreOrder(string orderId, UpdateInStoreOrderModel model);
    }
}
