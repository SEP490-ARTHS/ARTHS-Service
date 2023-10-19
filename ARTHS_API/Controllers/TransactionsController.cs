using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<TransactionViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all transactions")]
        public async Task<ActionResult<ListViewModel<TransactionViewModel>>> GetTransactions([FromQuery] PaginationRequestModel pagination)
        {
            return await _transactionService.GetTransactions(pagination);
        }
    }
}
