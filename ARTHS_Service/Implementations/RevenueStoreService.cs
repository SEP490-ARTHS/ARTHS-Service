using ARTHS_Data;
using ARTHS_Data.Models.Requests.Filters;
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

        public async Task<ListViewModel<RevenueStoreViewModel>> GetRevenues(RevenueFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _revenueStoreRepository.GetAll().AsQueryable();
            if (filter.Month.HasValue)
            {
                query = query.Where(revenue => revenue.TransactionDate.Month.Equals(filter.Month));
            }
            if (filter.Year.HasValue)
            {
                query = query.Where(revenue => revenue.TransactionDate.Year.Equals(filter.Year));
            }

            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
               .Skip(pagination.PageNumber * pagination.PageSize)
               .Take(pagination.PageSize);

            var revenues = await paginatedQuery
                .ProjectTo<RevenueStoreViewModel>(_mapper.ConfigurationProvider)
                .OrderByDescending(transaction => transaction.TransactionDate)
                .AsNoTracking()
                .ToListAsync();

            return new ListViewModel<RevenueStoreViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = revenues
            };
        }

        public async Task<RevenueStoreViewModel> GetRevenue(string Id)
        {
            return await _revenueStoreRepository.GetMany(transaction => transaction.Id.Equals(Id))
                .ProjectTo<RevenueStoreViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy revenue");
        }
    }
}
