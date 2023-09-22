﻿using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Implementations;
using ARTHS_Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace ARTHS_Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ARTHS_DBContext _context;

        private IAccountRepository _account = null!;
        private ICustomerRepository _customer = null!;
        private IAccountRoleRepository _accountRole = null!;
        private ICartRepository _cart = null!;
        private ICartItemRepository _cartItem = null!;

        public UnitOfWork(ARTHS_DBContext context)
        {
            _context = context;
        }

        public IAccountRepository Account
        {
            get { return _account ??= new AccountRepository(_context); }
        }

        public ICustomerRepository Customer
        {
            get { return _customer ??= new CustomerRepository(_context); }
        }

        public IAccountRoleRepository AccountRole
        {
            get { return _accountRole ??= new AccountRoleRepository(_context); }
        }

        public ICartRepository Cart
        {
            get { return _cart ??= new CartRepository(_context); }
        }

        public ICartItemRepository CartItem
        {
            get { return _cartItem ??= new CartItemRepository(_context); }
        }

        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

        public IDbContextTransaction Transaction()
        {
            return _context.Database.BeginTransaction();
        }
    }
}