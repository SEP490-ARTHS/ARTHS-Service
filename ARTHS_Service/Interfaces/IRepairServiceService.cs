using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IRepairServiceService
    {
        Task<RepairServiceViewModel> GetRepairService(Guid id);
        Task<ListViewModel<RepairServiceViewModel>> GetRepairServices(RepairServiceFilterModel filter, PaginationRequestModel pagination);
        Task<RepairServiceViewModel> CreateRepairService(CreateRepairServiceModel model);
        Task<RepairServiceViewModel> UpdateRepairService(Guid id, UpdateRepairServiceModel model);
        Task<RepairServiceViewModel> UpdateRepairServiceImage(Guid repairServiceId, UpdateImageModel model);
        Task RemoveRepairServieImage(Guid imageId);
    }
}
