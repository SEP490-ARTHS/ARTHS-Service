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
        //-----------------
        Task<int> SaveChanges();
        IDbContextTransaction Transaction();
    }
}
