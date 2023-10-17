using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IBookingService
    {
        Task<BookingSettingViewModel> GetBookingSetting();
        Task<BookingSettingViewModel> UpdateBookingSetting(UpdateBookingSettingModel model);
    }
}
