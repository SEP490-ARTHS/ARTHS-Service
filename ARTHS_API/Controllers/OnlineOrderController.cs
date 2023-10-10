using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/online-orders")]
    [ApiController]
    public class OnlineOrderController : ControllerBase
    {
        private readonly IOnlineOrderService _onlineOrderService;

        public OnlineOrderController(IOnlineOrderService onlineOrderService)
        {
            _onlineOrderService = onlineOrderService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OnlineOrderViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all order online.")]
        public async Task<ActionResult<List<OnlineOrderViewModel>>> GetOrders()
        {
            return await _onlineOrderService.GetOrders();
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(OnlineOrderViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get online order by id.")]
        public async Task<ActionResult<OnlineOrderViewModel>> GetOrder([FromRoute] Guid Id)
        {
            return await _onlineOrderService.GetOrder(Id);
        }

        [HttpGet]
        [Route("customer")]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(List<OnlineOrderViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all online order for the currently logged-in customer.")]
        public async Task<ActionResult<List<OnlineOrderViewModel>>> GetOrdersOfCurrentCustomer()
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            return await _onlineOrderService.GetOrdersByCustomerId(auth!.Id);
        }


        [HttpGet]
        [Route("customer/{id}")]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(List<OnlineOrderViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all online order by customer Id.")]
        public async Task<ActionResult<List<OnlineOrderViewModel>>> GetOrdersOfCurrentCustomer([FromRoute] Guid Id)
        {
            return await _onlineOrderService.GetOrdersByCustomerId(Id);
        }


        [HttpPost]
        [Route("carts/{id}")]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(OnlineOrderViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Create online order.")]
        public async Task<ActionResult<OnlineOrderViewModel>> CreateOrder([FromRoute] Guid Id, [FromBody] CreateOnlineOrderModel model)
        {
            var order = await _onlineOrderService.CreateOrder(Id, model);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(OnlineOrderViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]

        [SwaggerOperation(Summary = "Update online order.")]
        public async Task<ActionResult<OnlineOrderViewModel>> UpdateOrder([FromRoute] Guid Id, [FromBody] UpdateOnlineOrderModel model)
        {
            var order = await _onlineOrderService.UpdateOrder(Id, model);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }



    }
}
