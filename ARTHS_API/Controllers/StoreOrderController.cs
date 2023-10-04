using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/store-order")]
    [ApiController]
    public class StoreOrderController : ControllerBase
    {
        private readonly IInStoreOrderService _inStoreOrderService;

        public StoreOrderController(IInStoreOrderService inStoreOrderService)
        {
            _inStoreOrderService = inStoreOrderService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BasicInStoreOrderViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all order in store.")]
        public async Task<ActionResult<List<BasicInStoreOrderViewModel>>> GetInStoreOrders([FromQuery] InStoreOrderFilterModel model)
        {
            return await _inStoreOrderService.GetInStoreOrders(model);
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
        [ProducesResponseType(typeof(InStoreOrderViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get order in store by id.")]
        public async Task<ActionResult<InStoreOrderViewModel>> CreateOrder([FromBody] CreateInStoreOrderModel model)
        {
            var order = await _inStoreOrderService.CreateInStoreOrder(model);
            return CreatedAtAction(nameof(GetInStoreOrder), new { id = order.Id }, order);
        }
    }
}
