using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<AccountViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get all accounts.")]
        public async Task<ActionResult<List<AccountViewModel>>> GetAccounts([FromQuery] AccountFilterModel filter)
        {
            var accounts = await _accountService.GetAccounts(filter);
            return Ok(accounts);
        }
    }
}
