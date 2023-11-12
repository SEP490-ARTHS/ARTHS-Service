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
        private IRepairServiceRepository _repair = null!;
        private IDiscountRepository _discount = null!;
        private IMotobikeProductRepository _motobikeProduct = null!;
        private IImageRepository _image = null!;
        private IMotobikeProductPriceRepository _motobikeProductPrice = null!;
        private IOrderRepository _order = null!;
        private IOrderDetailRepository _orderDetail = null!;
        private IWarrantyRepository _warranty = null!;
        private IRevenueStoreRepository _revenueStore = null!;
        private IWarrantyHistoryRepository _warrantyHistory = null!;
        private IFeedbackProductRepository _feedbackProduct = null!;
        private IRepairBookingRepository _repairBooking = null!;
        private IDeviceTokenRepository _deviceToken = null!;
        private INotificationRepository _notification = null!;
        private IConfigurationRepository _configuration = null!;
        private IMaintenanceScheduleRepository _maintenanceSchedule = null!;
        private IFeedbackStaffRepository _feedbackStaff = null!;
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

        public IRepairServiceRepository RepairService
        {
            get { return _repair ??= new RepairServiceRepository(_context); }
        }

        public IDiscountRepository Discount
        {
            get { return _discount ??= new DiscountRepository(_context); }
        }

        public IImageRepository Image
        {
            get { return _image ??= new ImageRepository(_context); }
        }
        public IMotobikeProductRepository MotobikeProduct
        {
            get { return _motobikeProduct ??= new MotobikeProductRepository(_context); }
        }

        public IMotobikeProductPriceRepository MotobikeProductPrice
        {
            get { return _motobikeProductPrice ??= new MotobikeProductPriceRepository(_context); }
        }

        

        public IWarrantyRepository Warranty
        {
            get { return _warranty ??= new WarrantyRepository(_context); }

        }
        
        

        public IFeedbackProductRepository FeedbackProduct
        {
            get { return _feedbackProduct ??= new FeedbackProductRepository(_context); }
        }

        public IRepairBookingRepository RepairBooking
        {
            get { return _repairBooking ??= new RepairBookingRepository(_context); }
        }

        public IDeviceTokenRepository DeviceToken
        {
            get { return _deviceToken ??= new DeviceTokenRepository(_context); }
        }

        public INotificationRepository Notification
        {
            get { return _notification ??= new NotificationRepository(_context); }
        }

        public IOrderRepository Order
        {
            get { return _order ??= new OrderRepository(_context); }
        }
        public IOrderDetailRepository OrderDetail
        {
            get { return _orderDetail ??= new OrderDetailRepository(_context); }
        }

        public IRevenueStoreRepository RevenueStore
        {
            get { return _revenueStore ??= new RevenueStoreRepository(_context); }
        }

        public IConfigurationRepository Configuration
        {
            get { return _configuration ??= new ConfigurationRepository(_context); }
        }

        public IMaintenanceScheduleRepository MaintenanceSchedule
        {
            get { return _maintenanceSchedule ??= new MaintenanceScheduleRepository(_context); }
        }

        public IWarrantyHistoryRepository WarrantyHistory
        {
            get { return _warrantyHistory ??= new WarrantyHistoryRepository(_context); }
        }

        public IFeedbackStaffRepository FeedbackStaff
        {
            get { return _feedbackStaff ??= new FeedbackStaffRepository(_context); }
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
