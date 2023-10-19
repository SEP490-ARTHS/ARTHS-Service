using ARTHS_Data;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class TransactionService : BaseService, ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _transactionRepository = unitOfWork.Transactions;
        }

        public async Task<ListViewModel<TransactionViewModel>> GetTransactions(PaginationRequestModel pagination)
        {
            var query = _transactionRepository.GetAll();


            var listTransactions = query
                .ProjectTo<TransactionViewModel>(_mapper.ConfigurationProvider)
                .OrderByDescending(transaction => transaction.TransactionDate);
            var transactions = await listTransactions.Skip(pagination.PageNumber * pagination.PageSize).Take(pagination.PageSize).AsNoTracking().ToListAsync();
            var totalRow = await listTransactions.AsNoTracking().CountAsync();
            if(transactions != null || transactions !=null && transactions.Any())
            {
                return new ListViewModel<TransactionViewModel>
                {
                    Pagination = new PaginationViewModel
                    {
                        PageNumber = pagination.PageNumber,
                        PageSize = pagination.PageSize,
                        TotalRow = totalRow
                    },
                    Data = transactions
                };
            }
            return null!;
        }

    }
}
