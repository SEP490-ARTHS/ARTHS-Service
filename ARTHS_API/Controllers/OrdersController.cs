using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IInvoiceService _invoiceService;

        public OrdersController(IOrderService orderService, IInvoiceService invoiceService)
        {
            _orderService = orderService;
            _invoiceService = invoiceService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<OrderViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all order.")]
        public async Task<ActionResult<ListViewModel<OrderViewModel>>> GetOrders([FromQuery] OrderFilterModel filter, [FromQuery]PaginationRequestModel pagination)
        {
            return await _orderService.GetOrders(filter, pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(OrderViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get order by id.")]
        public async Task<ActionResult<OrderViewModel>> GetOrder([FromRoute] string Id)
        {
            return await _orderService.GetOrder(Id);
        }


        [HttpPost]
        [Route("offline")]
        [Authorize(UserRole.Teller)]
        [ProducesResponseType(typeof(OrderViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Create offline order.")]
        public async Task<ActionResult<OrderViewModel>> CreateOrderOffline([FromBody] CreateOrderOfflineModel model)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var result = await _orderService.CreateOrderOffline(auth!.Id, model);
            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }

        [HttpPut]
        [Route("offline/{id}")]
        [Authorize(UserRole.Teller)]
        [ProducesResponseType(typeof(OrderViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update offline order.")]
        public async Task<ActionResult<OrderViewModel>> UpdateOrderOffline([FromRoute] string Id, [FromBody] UpdateInStoreOrderModel model)
        {
            var result = await _orderService.UpdateOrderOffline(Id, model);
            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }

        [HttpPost]
        [Route("online")]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(OrderViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Create online order.")]
        public async Task<ActionResult<OrderViewModel>> CreateOrderOnline([FromBody] CreateOrderOnlineModel model)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var result = await _orderService.CreateOrderOnline(auth!.Id, model);
            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }

        [HttpPut]
        [Route("online/{id}")]
        [ProducesResponseType(typeof(OrderViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update online order.")]
        public async Task<ActionResult<OrderViewModel>> UpdateOrderOnline([FromRoute] string Id, [FromBody] UpdateOrderOnlineModel model)
        {
            var result = await _orderService.UpdateOrderOnline(Id, model);
            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }


        [HttpGet]
        [Route("generate-invoice/{id}")]
        [SwaggerOperation(Summary = "Generate bill for order.")]
        public async Task<ActionResult<string>> Get([FromRoute] string Id)
        {
            var result = await _invoiceService.GenerateInvoice(Id, "D:\\invoice");
            return Ok(result);
        }

    }
}
