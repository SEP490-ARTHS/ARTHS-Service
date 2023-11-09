using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/feedback-products")]
    [ApiController]
    public class FeedbackProductsController : ControllerBase
    {
        private readonly IFeedbackProductService _feedbackProductService;

        public FeedbackProductsController(IFeedbackProductService feedbackProductService)
        {
            _feedbackProductService = feedbackProductService;
        }


        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(FeedbackProductViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get feedback of product by id.")]
        public async Task<ActionResult<FeedbackProductViewModel>> GetFeedback([FromRoute] Guid Id)
        {
            return await _feedbackProductService.GetFeedback(Id);
        }


        [HttpPost]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(FeedbackProductViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Create feedback of product by customer.")]
        public async Task<ActionResult<FeedbackProductViewModel>> CreateProductFeedback([FromBody] CreateFeedbackProductModel model)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var productFeedback = await _feedbackProductService.CreateProductFeedback(auth!.Id, model);
            return CreatedAtAction(nameof(GetFeedback), new { id = productFeedback.Id }, productFeedback);
        }


        [HttpPut]
        [Route("{id}")]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(FeedbackProductViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Update feedback of product by customer.")]
        public async Task<ActionResult<FeedbackProductViewModel>> UpdateProductFeedback([FromRoute] Guid Id, [FromBody] UpdateFeedbackProductModel model)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var productFeedback = await _feedbackProductService.UpdateProductFeedback(auth!.Id, Id, model);
            return CreatedAtAction(nameof(GetFeedback), new { id = productFeedback.Id }, productFeedback);
        }
    }
}
