using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;

namespace ARTHS_Data.Repositories.Implementations
{
    public class CustomerRepository : Repository<CustomerAccount>, ICustomerRepository
    {
        public CustomerRepository(ARTHS_DBContext context) : base(context)
        {
        }
    }
}
