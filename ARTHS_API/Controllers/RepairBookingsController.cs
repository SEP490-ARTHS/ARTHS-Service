using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ARTHS_API.Controllers
{
    [Route("api/repair-bookings")]
    [ApiController]
    public class RepairBookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public RepairBookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        [Route("setting")]
        public async Task<ActionResult<BookingSettingViewModel>> GetBookingSetting()
        {
            return await _bookingService.GetBookingSetting();
        }

        [HttpPut]
        [Route("setting")]
        public async Task<ActionResult<BookingSettingViewModel>> UpdateBookingSetting([FromBody] UpdateBookingSettingModel model)
        {
            var setting = await _bookingService.UpdateBookingSetting(model);
            return CreatedAtAction(nameof(GetBookingSetting), setting);
        }
    }
}
