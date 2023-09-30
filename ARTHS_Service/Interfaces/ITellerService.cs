using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using Microsoft.AspNetCore.Http;

namespace ARTHS_Service.Interfaces
{
    public interface ITellerService
    {
        Task<TellerViewModel> GetTeller(Guid id);
        Task<TellerViewModel> CreateTeller(RegisterTellerModel model);
        Task<TellerViewModel> UpdateTeller(Guid id, UpdateTellerModel model);
        Task<TellerViewModel> UploadAvatar(Guid id, IFormFile image);
    }
}
