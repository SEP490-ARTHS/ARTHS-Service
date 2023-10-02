using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ARTHS_API.Controllers
{
    [Route("api/discount")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;
        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DiscountViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all discounts.")]
        public async Task<ActionResult<List<DiscountViewModel>>> GetDiscounts([FromQuery] DiscountFilterModel filter)
        {
            return await _discountService.GetDiscounts(filter);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(DiscountViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get discount by id.")]
        public async Task<ActionResult<DiscountViewModel>> GetDiscount([FromRoute] Guid id)
        {
            return await _discountService.GetDiscount(id);
        }

        [HttpPost]
        [Authorize(UserRole.Owner)]
        [ProducesResponseType(typeof(DiscountViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Create discount.")]
        public async Task<ActionResult<DiscountViewModel>> CreateDiscount([FromForm][Required] CreateDiscountModel model)
        {
            var Discount = await _discountService.CreateDiscount(model);
            //chuẩn REST
            return CreatedAtAction(nameof(GetDiscount), new { id = Discount.Id }, Discount);
        }

        [HttpPut]
        [Authorize(UserRole.Owner)]
        [Route("{id}")]
        [ProducesResponseType(typeof(DiscountViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update discount.")]
        public async Task<ActionResult<CustomerViewModel>> UpdateCustomer([FromRoute] Guid id, [FromForm] UpdateDiscountModel model)
        {
            var Discount = await _discountService.UpdateDiscount(id, model);
            return CreatedAtAction(nameof(GetDiscount), new { id = Discount.Id }, Discount);
        }
    }
}
