using ARTHS_Service.Implementations;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Helpers.Models;
using ARTHS_Utility.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/ghn")]
    [ApiController]
    public class GhnController : ControllerBase
    {
        private readonly IGhnService _ghnService;

        public GhnController(IGhnService ghnService)
        {
            _ghnService = ghnService;
        }

        [HttpPost]
        [Route("transfer-order")]
        [ProducesResponseType(typeof(GhnCreateResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Transfer of an order to GHN for shipping.")]
        public async Task<ActionResult<GhnCreateResponseModel>> CreateShipping([FromBody] GhnCreateOrderModel model)
        {
            var result = await _ghnService.CreateShippingOrder(model);
            return result;
        }

        [HttpPost]
        [Route("web-hook")]
        [SwaggerOperation(Summary = "Webhook callback from GHN for order updates.")]
        public async Task<ActionResult<GhnWebHookResponse>> GhnCallBack([FromBody] GhnWebHookResponse request)
        {
            await _ghnService.GhnCallBack(request);
            return Ok(request);
        }
    }
}
