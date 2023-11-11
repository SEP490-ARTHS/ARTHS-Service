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
    [Route("api/feedback")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackProductService;

        public FeedbackController(IFeedbackService feedbackProductService)
        {
            _feedbackProductService = feedbackProductService;
        }


        [HttpGet]
        [Route("product/{id}")]
        [ProducesResponseType(typeof(FeedbackProductViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get feedback of product by id.")]
        public async Task<ActionResult<FeedbackProductViewModel>> GetFeedbackProduct([FromRoute] Guid Id)
        {
            return await _feedbackProductService.GetFeedbackProduct(Id);
        }


        [HttpPost]
        [Route("product")]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(FeedbackProductViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Create feedback of product by customer.")]
        public async Task<ActionResult<FeedbackProductViewModel>> CreateProductFeedback([FromBody] CreateFeedbackProductModel model)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var productFeedback = await _feedbackProductService.CreateProductFeedback(auth!.Id, model);
            return CreatedAtAction(nameof(GetFeedbackProduct), new { id = productFeedback.Id }, productFeedback);
        }


        [HttpPut]
        [Route("product/{id}")]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(FeedbackProductViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Update feedback for product.")]
        public async Task<ActionResult<FeedbackProductViewModel>> UpdateProductFeedback([FromRoute] Guid Id, [FromBody] UpdateFeedbackProductModel model)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var productFeedback = await _feedbackProductService.UpdateProductFeedback(auth!.Id, Id, model);
            return CreatedAtAction(nameof(GetFeedbackProduct), new { id = productFeedback.Id }, productFeedback);
        }


        [HttpGet]
        [Route("staff/{id}")]
        [ProducesResponseType(typeof(FeedbackStaffViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get feedback of staff by id.")]
        public async Task<ActionResult<FeedbackStaffViewModel>> GetFeedbackStaff([FromRoute] Guid Id)
        {
            return await _feedbackProductService.GetFeedbackStaff(Id);
        }


        [HttpPost]
        [Route("staff")]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(FeedbackStaffViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Create feedback for staff.")]
        public async Task<ActionResult<FeedbackProductViewModel>> CreateFeedbackStaff([FromBody] CreateFeedbackStaffModel model)
        {
            //var auth = (AuthModel?)HttpContext.Items["User"];
            var staffFeedback = await _feedbackProductService.CreateFeedbackStaff( model);
            return CreatedAtAction(nameof(GetFeedbackStaff), new { id = staffFeedback.Id }, staffFeedback);
        }
    }
}
