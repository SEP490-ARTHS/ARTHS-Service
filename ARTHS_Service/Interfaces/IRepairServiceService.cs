using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IRepairServiceService
    {
        Task<RepairServiceViewModel> GetRepairService(Guid id);
        Task<List<RepairServiceViewModel>> GetRepairServices(RepairServiceFilterModel filter);
        Task<RepairServiceViewModel> CreateRepairService(CreateRepairServiceModel model);
        Task<RepairServiceViewModel> UpdateRepairService(Guid id, UpdateRepairServiceModel model);
    }
}
