using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class FeedbackProductRepository : Repository<FeedbackProduct>, IFeedbackProductRepository
    {
        public FeedbackProductRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
