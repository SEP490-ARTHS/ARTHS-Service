using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Enums;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ARTHS_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AuthViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Login.")]
        public async Task<IActionResult> AuthenticatedUser([FromBody][Required] AuthRequest auth)
        {
            var customer = await _authService.AuthenticatedUser(auth);
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
        [Authorize(Role.Staff, Role.Customer)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(Summary = "Get the currently logged-in user.")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = (AuthModel)HttpContext.Items["User"]!;
            if (user != null)
            {
                return Ok(await _authService.GetAccount(user.Id));
            }
            return new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }
    }
}
