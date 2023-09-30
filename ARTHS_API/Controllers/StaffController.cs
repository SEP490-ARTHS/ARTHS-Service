using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ARTHS_API.Controllers
{
    [Route("api/staffs")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IStaffService _staffService;

        public StaffController(IAccountService accountService, IStaffService staffService)
        {
            _accountService = accountService;
            _staffService = staffService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<AccountViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all staff accounts.")]
        public async Task<ActionResult<List<AccountViewModel>>> GetStaffs([FromQuery] AccountFilterModel filter)
        {
            var staffs = await _accountService.GetStaffs(filter);
            return Ok(staffs);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(StaffViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get staff by id.")]
        public async Task<ActionResult<StaffViewModel>> GetStaff([FromRoute] Guid id)
        {
            var staff = await _staffService.GetStaff(id);
            return staff != null ? Ok(staff) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(StaffViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Register staff.")]
        public async Task<ActionResult<StaffViewModel>> CreateStaff([FromBody][Required] RegisterStaffModel model)
        {
            var staff = await _staffService.CreateStaff(model);
            //chuẩn REST
            return CreatedAtAction(nameof(GetStaff), new { id = staff.AccountId }, staff);
        }


        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(StaffViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Update staff.")]
        public async Task<ActionResult<StaffViewModel>> UpdateStaff([FromRoute] Guid id, [FromBody] UpdateStaffModel model)
        {
            var staff = await _staffService.UpdateStaff(id, model);
            return CreatedAtAction(nameof(GetStaff), new { id = staff.AccountId }, staff);
        }

        [HttpPut]
        [Route("avatar")]
        [Authorize(UserRole.Staff)]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Upload avatar for staff.")]
        public async Task<ActionResult<CustomerViewModel>> UploadAvatar([Required] IFormFile image)
        {
            try
            {
                var auth = (AuthModel?)HttpContext.Items["User"];
                if (auth != null)
                {
                    var staff = await _staffService.UploadAvatar(auth.Id, image);
                    return CreatedAtAction(nameof(GetStaff), new { id = staff.AccountId }, staff);
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.InnerException != null ? e.InnerException.Message : e.Message);
            }
        }
    }
}
