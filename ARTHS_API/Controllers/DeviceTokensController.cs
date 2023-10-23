using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/device-tokens")]
    [ApiController]
    public class DeviceTokensController : ControllerBase
    {
        private readonly IDeviceTokenService _deviceTokenService;

        public DeviceTokensController(IDeviceTokenService deviceTokenService)
        {
            _deviceTokenService = deviceTokenService;
        }

        [HttpPost]
        [Authorize(UserRole.Staff, UserRole.Owner, UserRole.Teller, UserRole.Customer)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Create new device token for user.")]
        public async Task<ActionResult<bool>> CreateDeviceToken([FromBody] CreateDeviceTokenModel model)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            return await _deviceTokenService.CreateDeviceToken(auth!.Id, model);
        }
    }
}
