using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IBookingService
    {
        Task<List<RepairBookingViewModel>> GetRepairBookings();
        Task<RepairBookingViewModel> GetRepairBooking(Guid Id);
        Task<RepairBookingViewModel> CreateBooking(Guid customerId, CreateRepairBookingModel model);
        Task<RepairBookingViewModel> UpdateBooking(Guid repairBookingId, UpdateRepairBookingModel model);
        Task<BookingSettingViewModel> GetBookingSetting();
        Task<BookingSettingViewModel> UpdateBookingSetting(UpdateBookingSettingModel model);
    }
}
