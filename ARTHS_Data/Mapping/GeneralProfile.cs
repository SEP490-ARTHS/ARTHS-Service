using ARTHS_Data.Entities;
using ARTHS_Data.Models.Views;
using AutoMapper;

namespace ARTHS_Data.Mapping
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<Account, AccountViewModel>();
            CreateMap<AccountRole, RoleViewModel>();
            CreateMap<CustomerAccount, CustomerViewModel>();
        }
    }
}
