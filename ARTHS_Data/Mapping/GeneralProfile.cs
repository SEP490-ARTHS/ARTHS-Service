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

            CreateMap<Account, CustomerViewModel>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar));


            CreateMap<CustomerAccount, CustomerViewModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Account.Id))
                //.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Account.FullName))
                //.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber))
                .ForMember(dest => dest.Cart, opt => opt.MapFrom(src => src.Cart));

            CreateMap<Cart, CartViewModel>();
            CreateMap<CartItem, CartItemViewModel>();
        }
    }
}
