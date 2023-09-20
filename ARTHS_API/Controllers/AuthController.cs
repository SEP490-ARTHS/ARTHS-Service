using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ARTHS_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticatedUser([FromBody][Required] AuthRequest auth)
        {
            var customer = await _accountService.AuthenticatedUser(auth);
            if (customer != null)
            {
                return Ok(customer);
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
        }

        [HttpGet]
        [Authorize(Role.Customer, Role.Staff)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = (AuthModel)HttpContext.Items["User"]!;
            if (user != null)
            {
                return Ok(await _accountService.GetAccount(user.Id));
            }
            return new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }
    }
}
