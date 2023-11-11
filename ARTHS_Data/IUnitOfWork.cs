using ARTHS_Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace ARTHS_Data
{
    public interface IUnitOfWork
    {
        public IAccountRepository Account { get; }
        public ICustomerRepository Customer { get; }
        public IOwnerRepository Owner { get; }
        public ITellerRepository Teller { get; }
        public IStaffRepository Staff { get; }
        public IAccountRoleRepository AccountRole { get; }
        public ICartRepository Cart { get; }
        public ICartItemRepository CartItem { get; }
        public ICategoryRepository Category { get; }
        public IVehicleRepository Vehicle { get; }
        public IRepairServiceRepository RepairService { get; }
        public IDiscountRepository Discount { get; }
        public IImageRepository Image { get; }
        public IMotobikeProductRepository MotobikeProduct { get; }
        public IMotobikeProductPriceRepository MotobikeProductPrice { get; }
        public IOrderRepository Order { get; }
        public IOrderDetailRepository OrderDetail { get; }
        public IRevenueStoreRepository RevenueStore { get; }
        public IWarrantyRepository Warranty { get; }
        public IFeedbackProductRepository FeedbackProduct { get; }
        public IRepairBookingRepository RepairBooking { get; }
        public IDeviceTokenRepository DeviceToken { get; }
        public INotificationRepository Notification { get; }
        public IConfigurationRepository Configuration { get; }
        public IMaintenanceScheduleRepository MaintenanceSchedule { get; }
        public IWarrantyHistoryRepository WarrantyHistory { get; }
        public IFeedbackStaffRepository FeedbackStaff { get; }
        //-----------------
        Task<int> SaveChanges();
        IDbContextTransaction Transaction();
    }
}
