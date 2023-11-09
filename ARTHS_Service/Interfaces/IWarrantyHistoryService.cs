using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Data.Repositories.Interfaces
{
    public interface IWarrantyHistoryService
    {
        Task<ListViewModel<WarrantyHistoryViewModel>> GetWarrantyHistories(PaginationRequestModel pagination);
        Task<WarrantyHistoryViewModel> GetWarrantyHistory(Guid Id);
        Task<WarrantyHistoryViewModel> CreateWarranty(CreateWarrantyModel model);
        Task<WarrantyHistoryViewModel> UpdateWarranty(Guid Id, UpdateWarrantyModel model);
    }
}
