using ARTHS_Data;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Exceptions;
using ARTHS_Utility.Settings;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ARTHS_Service.Implementations
{
    public class BookingService : BaseService, IBookingService
    {
        private readonly BookingSetting _bookingSettings;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<BookingSetting> bookingSettings) : base(unitOfWork, mapper)
        {
            _bookingSettings = bookingSettings.Value;
        }

        public async Task<BookingSettingViewModel> GetBookingSetting()
        {
            return await Task.FromResult(new BookingSettingViewModel
            {
                TotalStaff = _bookingSettings.TotalStaff,
                WorkHours = _bookingSettings.WorkHours,
                ServiceTime = _bookingSettings.ServiceTime,
                NonBookingPercentage = _bookingSettings.NonBookingPercentage,
                DailyOnlineBookings = CalculateDailyOnlineBookings()
            });
        }


        public async Task<BookingSettingViewModel> UpdateBookingSetting(UpdateBookingSettingModel model)
        {
            if(model.TotalStaff == 0 || model.ServiceTime == 0 || model.WorkHours == 0)
            {
                throw new BadRequestException("Vui lòng nhập các giá trị total staff, service time, workHours khác 0");
            }

            _bookingSettings.TotalStaff = model.TotalStaff ?? _bookingSettings.TotalStaff;
            _bookingSettings.WorkHours = model.WorkHours ?? _bookingSettings.WorkHours;
            _bookingSettings.ServiceTime = model.ServiceTime ?? _bookingSettings.ServiceTime;
            _bookingSettings.NonBookingPercentage = model.NonBookingPercentage ?? _bookingSettings.NonBookingPercentage;

            return await GetBookingSetting();
        }

        private int CalculateDailyOnlineBookings()
        {
            int motosPerStaff = _bookingSettings.WorkHours / _bookingSettings.ServiceTime;
            int totalMotosPerDay = motosPerStaff * _bookingSettings.TotalStaff;
            return totalMotosPerDay * (100 - _bookingSettings.NonBookingPercentage) / 100;
        }

        //private void UpdateAppSettings()
        //{
        //    var section = _configuration.GetSection("BookingSetting");
        //    section["TotalStaff"] = _bookingSettings.TotalStaff.ToString();
        //    section["WorkHours"] = _bookingSettings.WorkHours.ToString();
        //    section["ServiceTime"] = _bookingSettings.ServiceTime.ToString();
        //    section["NonBookingPercentage"] = _bookingSettings.NonBookingPercentage.ToString();

        //    var json = JsonConvert.SerializeObject(_bookingSettings, Formatting.Indented);
        //    File.WriteAllText("appsettings.json", json);
        //}
    }
}
