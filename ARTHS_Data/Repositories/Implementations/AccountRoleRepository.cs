using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class AccountRoleRepository : Repository<AccountRole>, IAccountRoleRepository
    {
        public AccountRoleRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
