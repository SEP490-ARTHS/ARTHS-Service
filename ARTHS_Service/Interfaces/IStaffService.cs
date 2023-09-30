using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using Microsoft.AspNetCore.Http;

namespace ARTHS_Service.Interfaces
{
    public interface IStaffService
    {
        Task<StaffViewModel> GetStaff(Guid id);
        Task<StaffViewModel> CreateStaff(RegisterStaffModel model);
        Task<StaffViewModel> UpdateStaff(Guid id, UpdateStaffModel model);
        Task<StaffViewModel> UploadAvatar(Guid id, IFormFile image);
    }
}
