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
        public IInStoreOrderRepository InStoreOrder { get; }
        public IInStoreOrderDetailRepository InStoreOrderDetail { get; }
        public IOnlineOrderRepository OnlineOrder { get; }
        public IOnlineOrderDetailRepository OnlineOrderDetail { get; }
        public ITransactionRepository Transactions { get; }
        public IFeedbackProductRepository FeedbackProduct { get; }
        //-----------------
        Task<int> SaveChanges();
        IDbContextTransaction Transaction();
    }
}
