using ARTHS_Data.Models.Requests.Post;

namespace ARTHS_Service.Interfaces
{
    public interface IDeviceTokenService
    {
        Task<bool> CreateDeviceToken(Guid accountId, CreateDeviceTokenModel model);
    }
}
