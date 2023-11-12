using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using Microsoft.AspNetCore.Http;

namespace ARTHS_Service.Interfaces
{
    public interface IStaffService
    {
        Task<StaffDetailViewModel> GetStaff(Guid id);
        Task<StaffDetailViewModel> CreateStaff(RegisterStaffModel model);
        Task<StaffDetailViewModel> UpdateStaff(Guid id, UpdateStaffModel model);
        Task<StaffDetailViewModel> UploadAvatar(Guid id, IFormFile image);
    }
}
