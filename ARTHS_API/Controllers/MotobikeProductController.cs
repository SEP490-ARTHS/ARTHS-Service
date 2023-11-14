using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Implementations;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ARTHS_API.Controllers
{
    [Route("api/motobike-products")]
    [ApiController]
    public class MotobikeProductController : ControllerBase
    {
        private readonly IMotobikeProductService _motobikeProductService;

        public MotobikeProductController(IMotobikeProductService motobikeProductService)
        {
            _motobikeProductService = motobikeProductService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<MotobikeProductDetailViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all motobike products.")]
        public async Task<ActionResult<ListViewModel<MotobikeProductDetailViewModel>>> GetMotobikeProducts([FromQuery] MotobikeProductFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _motobikeProductService.GetMotobikeProducts(filter, pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(MotobikeProductDetailViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get motobike products by id.")]
        public async Task<ActionResult<MotobikeProductDetailViewModel>> GetMotobikeProduct([FromRoute] Guid id)
        {
            return await _motobikeProductService.GetMotobikeProduct(id);
        }

        [HttpPost]
        [Authorize(UserRole.Owner)]
        [ProducesResponseType(typeof(MotobikeProductDetailViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Create motobike product.")]
        public async Task<ActionResult<MotobikeProductDetailViewModel>> CreateMotobileProduct([FromForm][Required] CreateMotobikeProductModel model)
        {
            var motobikeProduct = await _motobikeProductService.CreateMotobikeProduct(model);
            return CreatedAtAction(nameof(GetMotobikeProduct), new { id = motobikeProduct.Id }, motobikeProduct);
        }

        [HttpPut]
        [Authorize(UserRole.Owner)]
        [Route("{id}")]
        [ProducesResponseType(typeof(MotobikeProductDetailViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update motobike product.")]
        public async Task<ActionResult<MotobikeProductDetailViewModel>> UpdateMotobileProduct([FromRoute] Guid id, [FromForm] UpdateMotobikeProductModel model)
        {
            var motobikeProduct = await _motobikeProductService.UpdateMotobikeProduct(id, model);
            return CreatedAtAction(nameof(GetMotobikeProduct), new { id = motobikeProduct.Id }, motobikeProduct);
        }

        [HttpPut]
        [Authorize(UserRole.Owner)]
        [Route("image/{id}")]
        [ProducesResponseType(typeof(MotobikeProductDetailViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update motobike product image.")]
        public async Task<ActionResult<MotobikeProductDetailViewModel>> UpdateMotobileProductImage([FromRoute] Guid id, [FromForm] UpdateImageModel model)
        {
            var motobikeProduct = await _motobikeProductService.UpdateMotobikeProductImage(id, model);
            return CreatedAtAction(nameof(GetMotobikeProduct), new { id = motobikeProduct.Id }, motobikeProduct);
        }

        [HttpDelete]
        [Authorize(UserRole.Owner)]
        [Route("image/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(Summary = "Remove motobike product image.")]
        public async Task<IActionResult> Remove([FromRoute] Guid id)
        {
            await _motobikeProductService.RemoveMotobikeProductImage(id);
            return NoContent();
        }
    }
}
