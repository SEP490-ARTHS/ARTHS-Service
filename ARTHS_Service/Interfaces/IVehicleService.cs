using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IVehicleService
    {
        public Task<VehicleViewModel> GetVehicle(Guid id);
        public Task<List<VehicleViewModel>> GetVehicles(VehicleFilterModel filter);
        public Task<VehicleViewModel> CreateVehicle(CreateVehicleRequest request);
        public Task<VehicleViewModel> UpdateVehicle(Guid Id, UpdateVehicleRequest request);
        public Task<VehicleViewModel> DeleteVehicle(Guid Id);
    }
}
