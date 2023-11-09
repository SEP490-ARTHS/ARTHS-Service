using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/warranties-history")]
    [ApiController]
    public class WarrantyHistoriesController : ControllerBase
    {
        private readonly IWarrantyHistoryService _warrantyHistoryService;

        public WarrantyHistoriesController(IWarrantyHistoryService warrantyHistoryService)
        {
            _warrantyHistoryService = warrantyHistoryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<WarrantyHistoryViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all warranties history of orders.")]
        public async Task<ActionResult<ListViewModel<WarrantyHistoryViewModel>>> GetWarranties([FromQuery]PaginationRequestModel pagination)
        {
            return await _warrantyHistoryService.GetWarrantyHistories(pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(WarrantyHistoryViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get warranty history of orders.")]
        public async Task<ActionResult<WarrantyHistoryViewModel>> GetWarranty([FromRoute] Guid id)
        {
            return await _warrantyHistoryService.GetWarrantyHistory(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(WarrantyHistoryViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Create warranty history for order.")]
        public async Task<ActionResult<WarrantyHistoryViewModel>> CreateWarranty([FromBody] CreateWarrantyModel model)
        {
            var result = await _warrantyHistoryService.CreateWarranty(model);
            return CreatedAtAction(nameof(GetWarranty), new { id = result.Id }, result);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(WarrantyHistoryViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update warranty history for order.")]
        public async Task<ActionResult<WarrantyHistoryViewModel>> UpdateWarranty([FromRoute]Guid Id, [FromBody] UpdateWarrantyModel model)
        {
            var result = await _warrantyHistoryService.UpdateWarranty(Id, model);
            return CreatedAtAction(nameof(GetWarranty), new { id = result.Id }, result);
        }




    }

}
