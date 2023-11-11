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
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber));

            CreateMap<OwnerAccount, OwnerViewModel>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber));

            CreateMap<TellerAccount, TellerViewModel>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber));

            CreateMap<StaffAccount, StaffViewModel>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber));
            CreateMap<StaffAccount, StaffDetailViewModel>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber));

            CreateMap<CustomerAccount, BasicCustomerViewModel>()
                .ForMember(dest => dest.PhoneNumber, otp => otp.MapFrom(src => src.Account.PhoneNumber));

            CreateMap<Cart, CartViewModel>();

            CreateMap<CartItem, CartItemViewModel>();

            CreateMap<Account, AccountViewModel>()
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

            CreateMap<RepairService, RepairServiceViewModel>()
                .ForMember(dest => dest.DiscountAmount, otp => otp.MapFrom(src => src.Discount != null ? src.Discount.DiscountAmount : 0));

            CreateMap<MotobikeProduct, MotobikeProductViewModel>()
                .ForMember(dest => dest.PriceCurrent, otp => otp.MapFrom(src => src.MotobikeProductPrices.OrderByDescending(price => price.CreateAt).FirstOrDefault()!.PriceCurrent))
                .ForMember(dest => dest.WarrantyDuration, otp => otp.MapFrom(src => src.Warranty != null ? src.Warranty.Duration : 0))
                .ForMember(dest => dest.DiscountAmount, otp => otp.MapFrom(src => src.Discount != null ? src.Discount.DiscountAmount : 0))
                .ForMember(dest => dest.ImageUrl, otp => otp.MapFrom(src => src.Images.FirstOrDefault()!.ImageUrl));

            CreateMap<MotobikeProduct, MotobikeProductDetailViewModel>()
                .ForMember(dest => dest.PriceCurrent, otp => otp.MapFrom(src => src.MotobikeProductPrices.OrderByDescending(price => price.CreateAt).FirstOrDefault()!.PriceCurrent))
                .ForMember(dest => dest.MotobikeProductPrices, otp => otp.MapFrom(src => src.MotobikeProductPrices.OrderByDescending(price => price.CreateAt)))
                .ForMember(dest => dest.WarrantyDuration, otp => otp.MapFrom(src => src.Warranty != null ? src.Warranty.Duration : 0));

            CreateMap<Image, ImageViewModel>();

            CreateMap<MotobikeProductPrice, MotobikeProductPriceViewModel>();


            CreateMap<MotobikeProduct, BasicMotobikeProductViewModel>()
                .ForMember(dest => dest.Image, otp => otp.MapFrom(src => src.Images.FirstOrDefault()!.ImageUrl))
                .ForMember(dest => dest.DiscountAmount, otp => otp.MapFrom(src => src.Discount != null ? src.Discount.DiscountAmount : 0));

            CreateMap<RepairService, BasicRepairServiceViewModel>()
                .ForMember(dest => dest.Image, otp => otp.MapFrom(src => src.Images.FirstOrDefault()!.ImageUrl))
                .ForMember(dest => dest.DiscountAmount, otp => otp.MapFrom(src => src.Discount != null ? src.Discount.DiscountAmount : 0));


            CreateMap<FeedbackProduct, FeedbackProductViewModel>();
            CreateMap<RepairBooking, RepairBookingViewModel>();
            CreateMap<Notification, NotificationViewModel>()
                .ForMember(notificationVM => notificationVM.Data, config => config.MapFrom(notification => new NotificationDataViewModel
                {
                    CreateAt = notification.SendDate,
                    IsRead = notification.IsRead,
                    Link = notification.Link,
                    Type = notification.Type
                }));
            CreateMap<Order, OrderViewModel>();
            CreateMap<Order, BasicOrderViewModel>();
            CreateMap<OrderDetail, OrderDetailViewModel>();
            CreateMap<RevenueStore, RevenueStoreViewModel>();
            CreateMap<WarrantyHistory, WarrantyHistoryViewModel>();
            CreateMap<Configuration, ConfigurationViewModel>();
            CreateMap<Discount, BasicDiscountViewModel>();
            CreateMap<FeedbackStaff, FeedbackStaffViewModel>();

            //------------------------------------------
            CreateMap<Category, CategoryViewModel>();
            CreateMap<Vehicle, VehicleViewModel>();
            CreateMap<Discount, DiscountViewModel>()
                .ForMember(dest => dest.RepairService, opt => opt.MapFrom(src => src.RepairServices));
            CreateMap<Warranty, WarrantyViewModel>();
            CreateMap<MotobikeProduct, DiscountViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DiscountId))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.Discount != null ? src.Discount.DiscountAmount : 0));
            CreateMap<RepairService, DiscountViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DiscountId))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.Discount != null ? src.Discount.DiscountAmount : 0));



        }
    }
}
