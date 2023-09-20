using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
