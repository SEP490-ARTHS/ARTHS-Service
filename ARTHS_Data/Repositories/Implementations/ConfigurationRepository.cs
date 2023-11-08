using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class ConfigurationRepository : Repository<Configuration>, IConfigurationRepository
    {
        public ConfigurationRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
