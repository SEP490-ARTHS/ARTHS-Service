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
        [ProducesResponseType(typeof(List<TransactionViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all transactions")]
        public async Task<ActionResult<List<TransactionViewModel>>> GetTransactions()
        {
            return await _transactionService.GetTransactions();
        }
    }
}
