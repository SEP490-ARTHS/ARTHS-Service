using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Helpers;
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
            return await _accountService.GetStaffs(filter);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(StaffDetailViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get staff by id.")]
        public async Task<ActionResult<StaffDetailViewModel>> GetStaff([FromRoute] Guid id)
        {
            return await _staffService.GetStaff(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(StaffDetailViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(StaffDetailViewModel), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Register staff.")]
        public async Task<ActionResult<StaffDetailViewModel>> CreateStaff([FromBody][Required] RegisterStaffModel model)
        {
            var staff = await _staffService.CreateStaff(model);
            //chuẩn REST
            return CreatedAtAction(nameof(GetStaff), new { id = staff.AccountId }, staff);
        }


        [HttpPut]
        [Route("{id}")]
        //[Authorize(UserRole.Staff)]
        [ProducesResponseType(typeof(StaffDetailViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update staff.")]
        public async Task<ActionResult<StaffDetailViewModel>> UpdateStaff([FromRoute] Guid id, [FromBody] UpdateStaffModel model)
        {
            var staff = await _staffService.UpdateStaff(id, model);
            return CreatedAtAction(nameof(GetStaff), new { id = staff.AccountId }, staff);
        }

        [HttpPut]
        [Route("avatar")]
        [Authorize(UserRole.Staff)]
        [ProducesResponseType(typeof(StaffDetailViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Upload avatar for staff.")]
        public async Task<ActionResult<StaffDetailViewModel>> UploadAvatar([Required] IFormFile image)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var staff = await _staffService.UploadAvatar(auth!.Id, image);
            return CreatedAtAction(nameof(GetStaff), new { id = staff.AccountId }, staff);
        }
    }
}
