using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionViewModel> GetTransaction(Guid id);
        Task<List<TransactionViewModel>> GetTransactions();
    }
}
