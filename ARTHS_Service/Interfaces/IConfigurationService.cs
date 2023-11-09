using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IConfigurationService
    {
        Task<ConfigurationViewModel> GetSetting();
        Task<ConfigurationViewModel> UpdateSetting(UpdateConfigurationModel model);
        Task<int> CalculateDailyOnlineBookings();
    }
}
