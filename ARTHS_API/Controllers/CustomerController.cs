using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Internal;
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get all customer accounts.")]
        public async Task<ActionResult<List<AccountViewModel>>> GetCustomers([FromQuery] AccountFilterModel filter)
        {
            var customers = await _accountService.GetCustomers(filter);
            return Ok(customers);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get customer by id.")]
        public async Task<ActionResult<CustomerViewModel>> GetCustomer([FromRoute] Guid id)
        {
            var customer = await _customerService.GetCustomer(id);
            return customer != null ? Ok(customer) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Upload avatar for customer.")]
        public async Task<ActionResult<CustomerViewModel>> UploadAvatar([Required] IFormFile image)
        {
            try
            {
                var auth = (AuthModel?)HttpContext.Items["User"];
                if (auth != null)
                {
                    var customer = await _customerService.UploadAvatar(auth.Id, image);
                    return CreatedAtAction(nameof(GetCustomer), new { id = customer.AccountId }, customer);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.InnerException != null ? ex.InnerException.Message : ex.Message });
            }
        }
    }
}
