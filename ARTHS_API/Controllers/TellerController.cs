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
            var tellers = await _accountService.GetTellers(filter);
            return Ok(tellers);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(TellerViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get teller by id.")]
        public async Task<ActionResult<TellerViewModel>> GetTeller([FromRoute] Guid id)
        {
            var teller = await _tellerService.GetTeller(id);
            return teller != null ? Ok(teller) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(TellerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Register teller.")]
        public async Task<ActionResult<TellerViewModel>> CreateOwner([FromBody][Required] RegisterTellerModel model)
        {
            try
            {
                var teller = await _tellerService.CreateTeller(model);
                //chuẩn REST
                return CreatedAtAction(nameof(GetTeller), new { id = teller.AccountId }, teller);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }


        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(TellerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Update teller.")]
        public async Task<ActionResult<TellerViewModel>> UpdateOwner([FromRoute] Guid id, [FromBody] UpdateTellerModel model)
        {
            try
            {
                var teller = await _tellerService.UpdateTeller(id, model);
                return CreatedAtAction(nameof(GetTeller), new { id = teller.AccountId }, teller);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        [HttpPut]
        [Route("avatar")]
        [Authorize(UserRole.Teller)]
        [ProducesResponseType(typeof(TellerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Upload avatar for teller.")]
        public async Task<ActionResult<TellerViewModel>> UploadAvatar([Required] IFormFile image)
        {
            try
            {
                var auth = (AuthModel?)HttpContext.Items["User"];
                if (auth != null)
                {
                    var teller = await _tellerService.UploadAvatar(auth.Id, image);
                    return CreatedAtAction(nameof(GetTeller), new { id = teller.AccountId }, teller);
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
