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
    [Route("api/store-orders")]
    [ApiController]
    public class StoreOrderController : ControllerBase
    {
        private readonly IInStoreOrderService _inStoreOrderService;

        public StoreOrderController(IInStoreOrderService inStoreOrderService)
        {
            _inStoreOrderService = inStoreOrderService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<BasicInStoreOrderViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all order in store.")]
        public async Task<ActionResult<ListViewModel<BasicInStoreOrderViewModel>>> GetInStoreOrders([FromQuery] InStoreOrderFilterModel model, [FromQuery] PaginationRequestModel pagination)
        {
            return await _inStoreOrderService.GetInStoreOrders(model, pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(InStoreOrderViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get order in store by id.")]
        public async Task<ActionResult<InStoreOrderViewModel>> GetInStoreOrder([FromRoute] string id)
        {
            return await _inStoreOrderService.GetInStoreOrder(id);
        }


        [HttpPost]
        [Authorize(UserRole.Teller)]
        [ProducesResponseType(typeof(InStoreOrderViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Create order in store.")]
        public async Task<ActionResult<InStoreOrderViewModel>> CreateOrder([FromBody] CreateInStoreOrderModel model)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var order = await _inStoreOrderService.CreateInStoreOrder(auth!.Id, model);
            return CreatedAtAction(nameof(GetInStoreOrder), new { id = order.Id }, order);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(UserRole.Teller)]
        [ProducesResponseType(typeof(InStoreOrderViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update in store order")]
        public async Task<ActionResult<InStoreOrderViewModel>> UpdateInStoreOrder([FromRoute] string Id, [FromBody] UpdateInStoreOrderModel model)
        {
            var order = await _inStoreOrderService.UpdateInStoreOrder(Id, model);
            return CreatedAtAction(nameof(GetInStoreOrder), new { id = order.Id }, order);
        }
    }
}
