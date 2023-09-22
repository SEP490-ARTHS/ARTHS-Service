using ARTHS_Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace ARTHS_Data
{
    public interface IUnitOfWork
    {
        public IAccountRepository Account { get; }
        public ICustomerRepository Customer { get; }
        public IAccountRoleRepository AccountRole { get; }
        public ICartRepository Cart { get; }
        public ICartItemRepository CartItem { get; }


        //-----------------
        Task<int> SaveChanges();
        IDbContextTransaction Transaction();
    }
}
