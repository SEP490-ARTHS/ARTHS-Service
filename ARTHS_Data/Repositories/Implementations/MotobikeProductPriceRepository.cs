using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class MotobikeProductPriceRepository : Repository<MotobikeProductPrice>, IMotobikeProductPriceRepository
    {
        public MotobikeProductPriceRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
