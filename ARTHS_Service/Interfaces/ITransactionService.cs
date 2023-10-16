using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface ITransactionService
    {
        Task<List<TransactionViewModel>> GetTransactions();
    }
}
