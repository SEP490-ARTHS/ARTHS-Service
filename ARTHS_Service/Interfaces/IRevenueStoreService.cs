using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IRevenueStoreService
    {
        Task<ListViewModel<RevenueStoreViewModel>> GetRevenues(PaginationRequestModel pagination);
        Task<RevenueStoreViewModel> GetRevenue(Guid Id);
    }
}
