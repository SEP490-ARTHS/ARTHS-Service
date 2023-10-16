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
    [Route("api/warranties")]
    [ApiController]
    public class WarrantyController : ControllerBase
    {
        private readonly IWarrantyService _WarrantyService;
        public WarrantyController(IWarrantyService warrantyService)
        {
            _WarrantyService = warrantyService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(WarrantyViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all warranties.")]
        public async Task<ActionResult<List<WarrantyViewModel>>> GetWarranties([FromQuery] WarrantyFilterModel filter)
        {
            return await _WarrantyService.GetWarranties(filter);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(WarrantyViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get warranty by id.")]
        public async Task<ActionResult<WarrantyViewModel>> GetWarranty([FromRoute] Guid id)
        {
            return await _WarrantyService.GetWarranty(id);
        }

        [HttpPost]
        [Authorize(UserRole.Owner)]
        [ProducesResponseType(typeof(WarrantyViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Create new warranty.")]
        public async Task<ActionResult<WarrantyViewModel>> CreateWarranty([FromForm][Required] CreateWarrantyRequest model)
        {
            var Warranty = await _WarrantyService.CreateWarranty(model);
            //chuẩn REST
            return CreatedAtAction(nameof(GetWarranty), new { id = Warranty.Id }, Warranty);
        }

        [HttpPut]
        [Authorize(UserRole.Owner)]
        [Route("{id}")]
        [ProducesResponseType(typeof(WarrantyViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update warranty.")]
        public async Task<ActionResult<CustomerViewModel>> UpdateCustomer([FromRoute] Guid id, [FromForm] UpdateWarrantyRequest model)
        {
            var Warranty = await _WarrantyService.UpdateWarranty(id, model);
            return CreatedAtAction(nameof(GetWarranty), new { id = Warranty.Id }, Warranty);
        }
    }
}
