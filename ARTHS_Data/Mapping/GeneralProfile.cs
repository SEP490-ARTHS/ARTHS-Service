using ARTHS_Data.Entities;
using ARTHS_Data.Models.Views;
using AutoMapper;

namespace ARTHS_Data.Mapping
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<AccountRole, RoleViewModel>();

            CreateMap<CustomerAccount, CustomerViewModel>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber))
                .ForMember(dest => dest.Cart, opt => opt.MapFrom(src => src.Cart));
            CreateMap<OwnerAccount, OwnerViewModel>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber));
            CreateMap<TellerAccount, TellerViewModel>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber));
            CreateMap<StaffAccount, StaffViewModel>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber));
            CreateMap<Cart, CartViewModel>();
            CreateMap<CartItem, CartItemViewModel>();
            CreateMap<Account, AccountViewModel>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Role, otp => otp.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.CustomerAccount != null ? src.CustomerAccount.FullName :
                                                            (src.OwnerAccount != null ? src.OwnerAccount.FullName :
                                                            (src.TellerAccount != null ? src.TellerAccount.FullName :
                                                            (src.StaffAccount != null ? src.StaffAccount.FullName : string.Empty)))))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.CustomerAccount != null ? src.CustomerAccount.Gender :
                                                            (src.OwnerAccount != null ? src.OwnerAccount.Gender :
                                                            (src.TellerAccount != null ? src.TellerAccount.Gender :
                                                            (src.StaffAccount != null ? src.StaffAccount.Gender : string.Empty)))))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.CustomerAccount != null ? src.CustomerAccount.Avatar :
                                                            (src.OwnerAccount != null ? src.OwnerAccount.Avatar :
                                                            (src.TellerAccount != null ? src.TellerAccount.Avatar :
                                                            (src.StaffAccount != null ? src.StaffAccount.Avatar : null)))));
            CreateMap<RepairService, RepairServiceViewModel>();
            CreateMap<MotobikeProduct, MotobikeProductViewModel>()
                .ForMember(dest => dest.PriceCurrent, otp => otp.MapFrom(src => src.MotobikeProductPrices.OrderByDescending(price => price.CreateAt).FirstOrDefault()!.PriceCurrent))
                .ForMember(dest => dest.WarrantyDuration, otp => otp.MapFrom(src => src.Warranty != null ? src.Warranty.Duration : 0));
            CreateMap<MotobikeProduct, MotobikeProductDetailViewModel>()
                .ForMember(dest => dest.PriceCurrent, otp => otp.MapFrom(src => src.MotobikeProductPrices.OrderByDescending(price => price.CreateAt).FirstOrDefault()!.PriceCurrent))
                .ForMember(dest => dest.MotobikeProductPrices, otp => otp.MapFrom(src => src.MotobikeProductPrices.OrderByDescending(price => price.CreateAt)))
                .ForMember(dest => dest.WarrantyDuration, otp => otp.MapFrom(src => src.Warranty != null ? src.Warranty.Duration : 0));
            CreateMap<Image, ImageViewModel>();
            CreateMap<MotobikeProductPrice, MotobikeProductPriceViewModel>();
            CreateMap<InStoreOrder, InStoreOrderViewModel>()
                .ForMember(dest => dest.TellerName, otp => otp.MapFrom(src => src.Teller.FullName))
                .ForMember(dest => dest.StaffName, otp => otp.MapFrom(src => src.Staff.FullName));
            CreateMap<InStoreOrder, BasicInStoreOrderViewModel>()
                .ForMember(dest => dest.TellerName, otp => otp.MapFrom(src => src.Teller.FullName))
                .ForMember(dest => dest.StaffName, otp => otp.MapFrom(src => src.Staff.FullName));
            CreateMap<MotobikeProduct, BasicMotobikeProductViewModel>()
                .ForMember(dest => dest.Image, otp => otp.MapFrom(src => src.Images.FirstOrDefault()!.ImageUrl));
            CreateMap<RepairService, BasicRepairServiceViewModel>()
                .ForMember(dest => dest.Image, otp => otp.MapFrom(src => src.Images.FirstOrDefault()!.ImageUrl));
            CreateMap<InStoreOrderDetail, InStoreOrderDetailViewModel>();

            //------------------------------------------
            CreateMap<Category, CategoryViewModel>();
            CreateMap<Vehicle, VehicleViewModel>();
            CreateMap<Discount, DiscountViewModel>();

            

        }
    }
}
