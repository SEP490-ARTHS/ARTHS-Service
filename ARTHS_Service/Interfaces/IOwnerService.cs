using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using Microsoft.AspNetCore.Http;

namespace ARTHS_Service.Interfaces
{
    public interface IOwnerService
    {
        Task<OwnerViewModel> GetOwner(Guid id);
        Task<OwnerViewModel> CreateOwner(RegisterOwnerModel model);
        Task<OwnerViewModel> UpdateOwner(Guid id, UpdateOwnerModel model);
        Task<OwnerViewModel> UploadAvatar(Guid id, IFormFile image);
    }
}
