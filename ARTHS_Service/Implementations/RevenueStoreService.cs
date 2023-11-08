using ARTHS_Data;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class RevenueStoreService : BaseService, IRevenueStoreService
    {
        private readonly IRevenueStoreRepository _revenueStoreRepository;
        public RevenueStoreService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _revenueStoreRepository = unitOfWork.RevenueStore;
        }

        public async Task<ListViewModel<RevenueStoreViewModel>> GetRevenues(PaginationRequestModel pagination)
        {
            var query = _revenueStoreRepository.GetAll();


            var listTransactions = query
                .ProjectTo<RevenueStoreViewModel>(_mapper.ConfigurationProvider)
                .OrderByDescending(transaction => transaction.TransactionDate);
            var transactions = await listTransactions.Skip(pagination.PageNumber * pagination.PageSize).Take(pagination.PageSize).AsNoTracking().ToListAsync();
            var totalRow = await listTransactions.AsNoTracking().CountAsync();
            if (transactions != null || transactions != null && transactions.Any())
            {
                return new ListViewModel<RevenueStoreViewModel>
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

        public async Task<RevenueStoreViewModel> GetRevenue(Guid Id)
        {
            return await _revenueStoreRepository.GetMany(transaction => transaction.Id.Equals(Id))
                .ProjectTo<RevenueStoreViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy revenue");
        }
    }
}
