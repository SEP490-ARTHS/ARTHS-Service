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
    [Route("api/tellers")]
    [ApiController]
    public class TellerController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITellerService _tellerService;

        public TellerController(IAccountService accountService, ITellerService tellerService)
        {
            _accountService = accountService;
            _tellerService = tellerService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<AccountViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all teller accounts.")]
        public async Task<ActionResult<List<AccountViewModel>>> GetTellers([FromQuery] AccountFilterModel filter)
        {
            return await _accountService.GetTellers(filter);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(TellerViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get teller by id.")]
        public async Task<ActionResult<TellerViewModel>> GetTeller([FromRoute] Guid id)
        {
            return await _tellerService.GetTeller(id);

        }

        [HttpPost]
        [ProducesResponseType(typeof(TellerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Register teller.")]
        public async Task<ActionResult<TellerViewModel>> CreateOwner([FromBody][Required] RegisterTellerModel model)
        {
            var teller = await _tellerService.CreateTeller(model);
            //chuẩn REST
            return CreatedAtAction(nameof(GetTeller), new { id = teller.AccountId }, teller);
        }


        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(TellerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update teller.")]
        public async Task<ActionResult<TellerViewModel>> UpdateOwner([FromRoute] Guid id, [FromBody] UpdateTellerModel model)
        {
            var teller = await _tellerService.UpdateTeller(id, model);
            return CreatedAtAction(nameof(GetTeller), new { id = teller.AccountId }, teller);
        }

        [HttpPut]
        [Route("avatar")]
        [Authorize(UserRole.Teller)]
        [ProducesResponseType(typeof(TellerViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Upload avatar for teller.")]
        public async Task<ActionResult<TellerViewModel>> UploadAvatar([Required] IFormFile image)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var teller = await _tellerService.UploadAvatar(auth!.Id, image);
            return CreatedAtAction(nameof(GetTeller), new { id = teller.AccountId }, teller);

        }
    }
}
