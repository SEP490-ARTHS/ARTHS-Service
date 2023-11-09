using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
        [ProducesResponseType(typeof(ListViewModel<RepairBookingViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all booking repair service.")]
        public async Task<ActionResult<ListViewModel<RepairBookingViewModel>>> GetRepairBookings([FromQuery] BookingFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _bookingService.GetRepairBookings(filter, pagination);
        }


        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(RepairBookingViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Get booking repair service of customer.")]
        public async Task<ActionResult<RepairBookingViewModel>> GetRepairBooking([FromRoute] Guid Id)
        {
            return await _bookingService.GetRepairBooking(Id);
        }

        [HttpPost]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(RepairBookingViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Create booking repair service for customer.")]
        public async Task<ActionResult<RepairBookingViewModel>> CreateBooking([FromBody] CreateRepairBookingModel model)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var booking = await _bookingService.CreateBooking(auth!.Id, model);
            return CreatedAtAction(nameof(GetRepairBooking), new { id = booking.Id }, booking);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(RepairBookingViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Update booking repair service for customer.")]
        public async Task<ActionResult<RepairBookingViewModel>> UpdateRepairBooking([FromRoute] Guid Id, [FromBody] UpdateRepairBookingModel model)
        {
            var booking = await _bookingService.UpdateBooking(Id, model);
            return CreatedAtAction(nameof(GetRepairBooking), new { id = booking.Id }, booking);
        }

        
    }
}
