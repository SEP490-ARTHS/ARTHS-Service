using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/revenues")]
    [ApiController]
    public class RevenuesController : ControllerBase
    {
        private readonly IRevenueStoreService _revenueStoreService;

        public RevenuesController(IRevenueStoreService revenueStoreService)
        {
            _revenueStoreService = revenueStoreService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<RevenueStoreViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all revenues")]
        public async Task<ActionResult<ListViewModel<RevenueStoreViewModel>>> GetRevenues([FromQuery] RevenueFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _revenueStoreService.GetRevenues(filter, pagination);
        }
    }
}
