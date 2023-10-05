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
    [Route("api/owners")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IOwnerService _ownerService;

        public OwnerController(IAccountService accountService, IOwnerService ownerService)
        {
            _accountService = accountService;
            _ownerService = ownerService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<AccountViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all owner accounts.")]
        public async Task<ActionResult<List<AccountViewModel>>> GetOwners([FromQuery] AccountFilterModel filter)
        {
            return await _accountService.GetOwners(filter);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(OwnerViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get owner by id.")]
        public async Task<ActionResult<OwnerViewModel>> GetOwner([FromRoute] Guid id)
        {
            return await _ownerService.GetOwner(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OwnerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Register owner.")]
        public async Task<ActionResult<OwnerViewModel>> CreateOwner([FromBody][Required] RegisterOwnerModel model)
        {
            var owner = await _ownerService.CreateOwner(model);
            //chuẩn REST
            return CreatedAtAction(nameof(GetOwner), new { id = owner.AccountId }, owner);
        }


        [HttpPut]
        [Route("{id}")]
        [Authorize(UserRole.Owner)]
        [ProducesResponseType(typeof(OwnerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update owner.")]
        public async Task<ActionResult<OwnerViewModel>> UpdateOwner([FromRoute] Guid id, [FromBody] UpdateOwnerModel model)
        {
            var owner = await _ownerService.UpdateOwner(id, model);
            return CreatedAtAction(nameof(GetOwner), new { id = owner.AccountId }, owner);
        }

        [HttpPut]
        [Route("avatar")]
        [Authorize(UserRole.Owner)]
        [ProducesResponseType(typeof(OwnerViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Upload avatar for owner.")]
        public async Task<ActionResult<OwnerViewModel>> UploadAvatar([Required] IFormFile image)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var owner = await _ownerService.UploadAvatar(auth!.Id, image);
            return CreatedAtAction(nameof(GetOwner), new { id = owner.AccountId }, owner);
        }
    }
}
