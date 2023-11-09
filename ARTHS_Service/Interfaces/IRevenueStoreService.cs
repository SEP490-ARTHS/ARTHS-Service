using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IRevenueStoreService
    {
        Task<ListViewModel<RevenueStoreViewModel>> GetRevenues(RevenueFilterModel filter, PaginationRequestModel pagination);
        Task<RevenueStoreViewModel> GetRevenue(string Id);
    }
}
