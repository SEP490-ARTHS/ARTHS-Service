﻿using ARTHS_Data;
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

        public async Task<List<TransactionViewModel>> GetTransactions()
        {
            var query = _transactionRepository.GetAll();

            return await query
                .ProjectTo<TransactionViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

    }
}