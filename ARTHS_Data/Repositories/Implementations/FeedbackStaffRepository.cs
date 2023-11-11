using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class FeedbackStaffRepository : Repository<FeedbackStaff>, IFeedbackStaffRepository
    {
        public FeedbackStaffRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
