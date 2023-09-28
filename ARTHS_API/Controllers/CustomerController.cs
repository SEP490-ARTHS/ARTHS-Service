using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ARTHS_API.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
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
            try
            {
                var customer = await _customerService.CreateCustomer(model);
                //chuẩn REST
                return CreatedAtAction(nameof(GetCustomer), new { id = customer.AccountId }, customer);
            }catch (Exception ex)
            {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Update cusomer.")]
        public async Task<ActionResult<CustomerViewModel>> UpdateCustomer([FromRoute]Guid id, [FromBody] UpdateCustomerModel model)
        {
            try
            {
                var customer = await _customerService.UpdateCustomer(id, model);
                return CreatedAtAction(nameof(GetCustomer), new { id = customer.AccountId }, customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}
