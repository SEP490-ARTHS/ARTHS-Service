using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ARTHS_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICustomerService _customerService;
        private readonly IOwnerService _ownerService;
        private readonly IStaffService _staffService;
        private readonly ITellerService _tellerService;

        public AuthController(IAuthService authService, ICustomerService customerService, IOwnerService ownerService, IStaffService staffService, ITellerService tellerService)
        {
            _authService = authService;
            _customerService = customerService;
            _ownerService = ownerService;
            _staffService = staffService;
            _tellerService = tellerService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AuthViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Login.")]
        public async Task<IActionResult> AuthenticatedUser([FromBody][Required] AuthRequest auth)
        {
            var customer = await _authService.AuthenticatedUser(auth);
            if (customer != null)
            {
                return Ok(customer);
            }
            else
            {
                return BadRequest(new { message = "Not found this account." });
            }
        }

        [HttpGet]
        [Authorize(UserRole.Customer)]
        [Route("customers")]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get details of the authenticated customer.")]
        public async Task<ActionResult<CustomerViewModel>> GetCustomer()
        {
            try
            {
                var auth = (AuthModel?)HttpContext.Items["User"];
                if (auth != null)
                {
                    var customer = await _customerService.GetCustomer(auth.Id);
                    return customer != null ? Ok(customer) : NotFound();
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.InnerException != null ? e.InnerException.Message : e.Message);
            }
            
        }

        [HttpGet]
        [Authorize(UserRole.Staff)]
        [Route("staffs")]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get details of the authenticated staff.")]
        public async Task<ActionResult<CustomerViewModel>> GetStaff()
        {
            try
            {
                var auth = (AuthModel?)HttpContext.Items["User"];
                if (auth != null)
                {
                    var customer = await _staffService.GetStaff(auth.Id);
                    return customer != null ? Ok(customer) : NotFound();
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.InnerException != null ? e.InnerException.Message : e.Message);
            }

        }

        [HttpGet]
        [Authorize(UserRole.Owner)]
        [Route("owners")]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get details of the authenticated owner.")]
        public async Task<ActionResult<CustomerViewModel>> GetOwner()
        {
            try
            {
                var auth = (AuthModel?)HttpContext.Items["User"];
                if (auth != null)
                {
                    var customer = await _ownerService.GetOwner(auth.Id);
                    return customer != null ? Ok(customer) : NotFound();
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.InnerException != null ? e.InnerException.Message : e.Message);
            }

        }

        [HttpGet]
        [Authorize(UserRole.Teller)]
        [Route("tellers")]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get details of the authenticated teller.")]
        public async Task<ActionResult<CustomerViewModel>> GetTeller()
        {
            try
            {
                var auth = (AuthModel?)HttpContext.Items["User"];
                if (auth != null)
                {
                    var customer = await _tellerService.GetTeller(auth.Id);
                    return customer != null ? Ok(customer) : NotFound();
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.InnerException != null ? e.InnerException.Message : e.Message);
            }

        }


    }
}
