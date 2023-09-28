using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class OwnerRepository : Repository<OwnerAccount>, IOwnerRepository
    {
        public OwnerRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
