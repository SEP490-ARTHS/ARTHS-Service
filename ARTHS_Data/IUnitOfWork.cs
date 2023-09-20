using ARTHS_Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace ARTHS_Data
{
    public interface IUnitOfWork
    {
        public IAccountRepository Account { get; }


        //-----------------
        Task<int> SaveChanges();
        IDbContextTransaction Transaction();
    }
}
