using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class TellerRepository : Repository<TellerAccount>, ITellerRepository
    {
        public TellerRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
