using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class ImageRepository : Repository<Image>, IImageRepository
    {
        public ImageRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
