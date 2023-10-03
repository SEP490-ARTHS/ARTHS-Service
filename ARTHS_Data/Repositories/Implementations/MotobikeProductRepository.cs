using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class MotobikeProductRepository : Repository<MotobikeProduct>, IMotobikeProductRepository
    {
        public MotobikeProductRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
