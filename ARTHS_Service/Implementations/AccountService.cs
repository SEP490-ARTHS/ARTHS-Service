using ARTHS_Data;
using ARTHS_Service.Interfaces;
using AutoMapper;

namespace ARTHS_Service.Implementations
{
    public class AccountService : BaseService, IAccountService
    {
        public AccountService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
