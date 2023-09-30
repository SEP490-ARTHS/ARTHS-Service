using ARTHS_Data.Entities;
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
        private IOwnerRepository _owner = null!;
        private ITellerRepository _teller = null!;
        private IStaffRepository _staff = null!;
        private IAccountRoleRepository _accountRole = null!;
        private ICartRepository _cart = null!;
        private ICartItemRepository _cartItem = null!;
        private ICategoryRepository _category = null!;
        private IVehicleRepository _vehicle = null!;

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

        public IOwnerRepository Owner
        {
            get { return _owner ??= new OwnerRepository(_context); }
        }

        public ITellerRepository Teller
        {
            get { return _teller ??= new TellerRepository(_context); }
        }

        public IStaffRepository Staff
        {
            get { return _staff ??= new StaffRepository(_context); }
        }

        public ICategoryRepository Category
        {
            get { return _category ??= new CategoryRepository(_context); }
        }

        public IVehicleRepository Vehicle
        {
            get { return _vehicle ??= new VehicleRepository(_context); }
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
