using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/carts")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(CartViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get cart by customer Id")]
        public async Task<ActionResult<CartViewModel>> GetCart()
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            return await _cartService.GetCartByCustomerId(auth!.Id);
        }

        [HttpPost]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(CartViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Add product to cart")]
        public async Task<ActionResult<CartViewModel>> AddToCart(CreateCartModel model)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var cart = await _cartService.AddToCart(auth!.Id, model);
            return CreatedAtAction(nameof(GetCart), cart);
        }

        [HttpPut]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(CartViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update cart")]
        public async Task<ActionResult<CartViewModel>> UpdateCart(UpdateCartModel model)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var cart = await _cartService.UpdateCart(auth!.Id, model);
            return CreatedAtAction(nameof(GetCart), cart);
        }
    }

}
