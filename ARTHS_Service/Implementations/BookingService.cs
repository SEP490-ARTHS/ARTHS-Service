using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Exceptions;
using ARTHS_Utility.Settings;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace ARTHS_Service.Implementations
{
    public class BookingService : BaseService, IBookingService
    {
        private readonly IRepairBookingRepository _repairBookingRepository;

        private readonly BookingSetting _bookingSettings;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<BookingSetting> bookingSettings) : base(unitOfWork, mapper)
        {
            _repairBookingRepository = unitOfWork.RepairBooking;

            _bookingSettings = bookingSettings.Value;
        }

        public async Task<List<RepairBookingViewModel>> GetRepairBookings()
        {
            var query = _repairBookingRepository.GetAll();



            return await query
                .ProjectTo<RepairBookingViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<RepairBookingViewModel> GetRepairBooking(Guid Id)
        {
            return await _repairBookingRepository.GetMany(booking => booking.Id.Equals(Id))
                .ProjectTo<RepairBookingViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy booking");
        }

        public async Task<RepairBookingViewModel> CreateBooking(Guid customerId, CreateRepairBookingModel model)
        {
            DateTime dateBook;
            if(!DateTime.TryParseExact(model.DateBook, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateBook))
            {
                throw new ConflictException("Vui lòng nhập đúng định dạng ngày (dd-MM-yyyy).");
            }
            if(!await IsBookingAvailableForDate(dateBook))
            {
                throw new ConflictException($"Cửa hàng đã đủ đơn cho ngày bạn chọn. Vui lòng chọn ngày khác.");
            }
            var booking = new RepairBooking
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                DateBook = dateBook,
                Description = model.Description,
                Status = RepairBookingStatus.WaitForConfirm
            };
            _repairBookingRepository.Add(booking);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetRepairBooking(booking.Id) : null!;
        }

        public async Task<RepairBookingViewModel> UpdateBooking(Guid repairBookingId, UpdateRepairBookingModel model)
        {
            var booking = await _repairBookingRepository.GetMany(booking => booking.Id.Equals(repairBookingId)).FirstOrDefaultAsync();
            if (booking == null) throw new NotFoundException("Không tìm thấy thông tin booking.");

            if(model.Status == RepairBookingStatus.Canceled && string.IsNullOrEmpty(model.CancellationReason))
            {
                throw new BadRequestException("Cần phải cung cấp lý do khi hủy lịch đặt.");
            }
            if (model.Status != null && model.Status.Equals(RepairBookingStatus.Canceled))
            {
                booking.CancellationReason = model.CancellationReason;
                booking.CancellationDate = DateTime.UtcNow;
                
            }

            if (!string.IsNullOrEmpty(model.TimeBook))
            {
                DateTime dateBooking = booking.DateBook;
                if (!string.IsNullOrEmpty(model.DateBook))
                {
                    DateTime newDateBook;
                    if (!DateTime.TryParseExact(model.DateBook, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out newDateBook))
                    {
                        throw new ConflictException("Vui lòng nhập đúng định dạng ngày (dd-MM-yyyy).");
                    }
                    if (!newDateBook.Date.Equals(dateBooking.Date))
                    {
                        dateBooking = newDateBook;
                        if (!await IsBookingAvailableForDate(dateBooking))
                        {
                            throw new ConflictException($"Cửa hàng đã đủ đơn cho ngày bạn chọn. Vui lòng chọn ngày khác.");
                        }
                    }

                }

                var timeBook = HandleTimeBooking(model.TimeBook);
                booking.DateBook = dateBooking.Date + timeBook;
            }
            
            booking.Description = model.Description ?? booking.Description;
            booking.Status = model.Status ?? booking.Status;

            _repairBookingRepository.Update(booking);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetRepairBooking(repairBookingId) : null!;
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


        private async Task<bool> IsBookingAvailableForDate(DateTime dateBook)
        {
            var booking = await _repairBookingRepository.GetMany(booking => booking.DateBook.Date.Equals(dateBook.Date) && !booking.Status.Equals(RepairBookingStatus.Canceled)).ToListAsync();
            if (booking.Count >= CalculateDailyOnlineBookings())
            {
                return false;
            }
            return true;
        }
        private TimeSpan HandleTimeBooking(string timeBook)
        {
            TimeSpan result;
            if (TimeSpan.TryParseExact(timeBook, "hh\\:mm\\:ss", null, out result))
            {
                if (result >= TimeSpan.Parse("08:00:00") && result <= TimeSpan.Parse("15:00:00"))
                {
                    return result;
                }
                throw new ConflictException("Thời nhận sửa chữa của cửa hàng là từ 8h - 15h");
            }
            else
            {
                throw new ConflictException("Vui lòng nhập đúng định dạng thời gian (hh:mm:ss)");
            }
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
