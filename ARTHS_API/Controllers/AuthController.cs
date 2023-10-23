using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
        [ProducesResponseType(typeof(TokenViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Login.")]
        public async Task<IActionResult> Authenticated([FromBody][Required] AuthRequest auth)
        {
            var token = await _authService.Authenticated(auth);

            //set cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                MaxAge = TimeSpan.FromDays(1),
            };
            Response.Cookies.Append("accessToken", token.AccessToken, cookieOptions);

            return Ok(token);
        }

        [HttpPost]
        [Route("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Logout.")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("accessToken");
            return Ok(new { message = "Successfully logged out" });
        }

        [HttpPost]
        [Route("refresh-token")]
        [Authorize(UserRole.Owner, UserRole.Teller, UserRole.Staff, UserRole.Customer)]
        [ProducesResponseType(typeof(TokenViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(Summary = "Refresh token.")]
        public async Task<ActionResult<TokenViewModel>> RefreshAuthentication()
        {
            var currentToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            var token = await _authService.RefreshAuthentication(currentToken!);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                MaxAge = TimeSpan.FromDays(1),
            };
            Response.Cookies.Append("accessToken", token.AccessToken, cookieOptions);

            return token;
        }

        [HttpGet]
        [Authorize(UserRole.Owner, UserRole.Teller, UserRole.Staff, UserRole.Customer)]
        [ProducesResponseType(typeof(AccountViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(Summary = "Retrieve logged-in account details.")]
        public async Task<ActionResult<AccountViewModel>> GetAccount()
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            return await _authService.GetAccount(auth!.Id);
        }
    }
}
