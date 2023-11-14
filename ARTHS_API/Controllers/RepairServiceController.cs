using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Implementations;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ARTHS_API.Controllers
{
    [Route("api/repair-services")]
    [ApiController]
    public class RepairServiceController : ControllerBase
    {
        private readonly IRepairServiceService _repairServiceService;

        public RepairServiceController(IRepairServiceService repairServiceService)
        {
            _repairServiceService = repairServiceService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<RepairServiceViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all repair services.")]
        public async Task<ActionResult<ListViewModel<RepairServiceViewModel>>> GetRepairServices([FromQuery] RepairServiceFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _repairServiceService.GetRepairServices(filter, pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(RepairServiceViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get repair service by id.")]
        public async Task<ActionResult<RepairServiceViewModel>> GetRepairService([FromRoute] Guid id)
        {
            return await _repairServiceService.GetRepairService(id);
        }

        [HttpPost]
        [Authorize(UserRole.Owner)]
        [ProducesResponseType(typeof(RepairServiceViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Create repair service.")]
        public async Task<ActionResult<RepairServiceViewModel>> CreateRepairService([FromForm][Required] CreateRepairServiceModel model)
        {
            var repairService = await _repairServiceService.CreateRepairService(model);
            //chuẩn REST
            return CreatedAtAction(nameof(GetRepairService), new { id = repairService.Id }, repairService);
        }

        [HttpPut]
        [Authorize(UserRole.Owner)]
        [Route("{id}")]
        [ProducesResponseType(typeof(RepairServiceViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update repair service.")]
        public async Task<ActionResult<RepairServiceViewModel>> UpdateRepairService([FromRoute] Guid id, [FromForm] UpdateRepairServiceModel model)
        {
            var repairService = await _repairServiceService.UpdateRepairService(id, model);
            return CreatedAtAction(nameof(GetRepairService), new { id = repairService.Id }, repairService);
        }

        [HttpPut]
        [Authorize(UserRole.Owner)]
        [Route("image/{id}")]
        [ProducesResponseType(typeof(RepairServiceViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update repair service image.")]
        public async Task<ActionResult<MotobikeProductDetailViewModel>> UpdateMotobileProductImage([FromRoute] Guid id, [FromForm] UpdateImageModel model)
        {
            var repairService = await _repairServiceService.UpdateRepairServiceImage(id, model);
            return CreatedAtAction(nameof(GetRepairService), new { id = repairService.Id }, repairService);
        }

        [HttpDelete]
        [Authorize(UserRole.Owner)]
        [Route("image/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(Summary = "Remove repair service image.")]
        public async Task<IActionResult> Remove([FromRoute] Guid id)
        {
            await _repairServiceService.RemoveRepairServieImage(id);
            return NoContent();
        }
    }
}
