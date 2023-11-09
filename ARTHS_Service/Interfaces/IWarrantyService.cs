using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IWarrantyService
    {
        public Task<List<WarrantyViewModel>> GetWarranties(WarrantyFilterModel filter);
        public Task<WarrantyViewModel> GetWarranty(Guid id);
        public Task<WarrantyViewModel> CreateWarranty(CreateWarrantyRequest model);
        public Task<WarrantyViewModel> UpdateWarranty(Guid id, UpdateWarrantyRequest model);
    }
}
