using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface ITransactionService
    {
        Task<ListViewModel<TransactionViewModel>> GetTransactions(PaginationRequestModel pagination);
    }
}
