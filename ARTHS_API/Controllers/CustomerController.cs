using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ARTHS_API.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;


        public CustomerController(ICustomerService customerService, IAccountService accountService)
        {
            _customerService = customerService;
            _accountService = accountService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<AccountViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all customer accounts.")]
        public async Task<ActionResult<List<AccountViewModel>>> GetCustomers([FromQuery] AccountFilterModel filter)
        {
            return await _accountService.GetCustomers(filter);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get customer by id.")]
        public async Task<ActionResult<CustomerViewModel>> GetCustomer([FromRoute] Guid id)
        {
            return await _customerService.GetCustomer(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Register cusomer.")]
        public async Task<ActionResult<CustomerViewModel>> CreateCustomer([FromBody][Required] RegisterCustomerModel model)
        {
            var customer = await _customerService.CreateCustomer(model);
            //chuẩn REST
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.AccountId }, customer);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update customer.")]
        public async Task<ActionResult<CustomerViewModel>> UpdateCustomer([FromRoute] Guid id, [FromBody] UpdateCustomerModel model)
        {
            var customer = await _customerService.UpdateCustomer(id, model);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.AccountId }, customer);
        }

        [HttpPut]
        [Route("avatar")]
        [Authorize(UserRole.Customer)]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Upload avatar for customer.")]
        public async Task<ActionResult<CustomerViewModel>> UploadAvatar([Required] IFormFile image)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var customer = await _customerService.UploadAvatar(auth!.Id, image);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.AccountId }, customer);
        }
    }
}
