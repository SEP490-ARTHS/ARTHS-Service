using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IRepairServiceService
    {
        Task<RepairServiceDetailViewModel> GetRepairService(Guid id);
        Task<ListViewModel<RepairServiceViewModel>> GetRepairServices(RepairServiceFilterModel filter, PaginationRequestModel pagination);
        Task<RepairServiceDetailViewModel> CreateRepairService(CreateRepairServiceModel model);
        Task<RepairServiceDetailViewModel> UpdateRepairService(Guid id, UpdateRepairServiceModel model);
    }
}
