using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Implementations;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Enums;
using ARTHS_Utility.Exceptions;
using ARTHS_Utility.Helpers;
using ARTHS_Utility.Helpers.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class GhnService : BaseService, IGhnService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly INotificationService _notificationService;
        public GhnService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService) : base(unitOfWork, mapper)
        {
            _orderRepository = unitOfWork.Order;
            _notificationService = notificationService;
        }

        public async Task<GhnCreateResponseModel> CreateShippingOrder(GhnCreateOrderModel model)
        {
            var order = await _orderRepository.GetMany(order => order.Id.Equals(model.OrderId))
                .Include(order => order.Customer)
                    .ThenInclude(customer => customer!.Account)
                .Include(order => order.OrderDetails)
                    .ThenInclude(detail => detail.MotobikeProduct)
                .FirstOrDefaultAsync();
            if (order == null)
            {
                throw new NotFoundException("Không tìm thấy order");
            }
            if (!order.Status.Equals(OrderStatus.Confirm) && !order.Status.Equals(OrderStatus.Paid))
            {
                throw new BadRequestException("Vui lòng kiểm tra lại thông tin. Hiện tại chưa thể bàn giao đơn hàng này hoặc đơn hàng này đã được bàn giao rồi.");
            }

            List<Item> items = new List<Item>();
            foreach (var item in order!.OrderDetails)
            {
                var product = new Item
                {
                    Name = item.MotobikeProduct!.Name,
                    Code = item.MotobikeProductId.ToString()!,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Length = 10,
                    Weight = 200,
                    Width = 30,
                    Height = 40,
                };
                items.Add(product);
            }
            var segments = order.Address!.Split(',');
            var requestData = new GhnCreateRequestModel
            {
                Note = model.Note,
                ToName = order.Customer!.FullName,
                ToPhone = order.CustomerPhoneNumber,
                ToAddress = segments[0].Trim(), //số nhà đường
                ToWardName = segments[1].Trim(), //quận
                ToDistrictName = segments[2].Trim(), // huyện
                ToProvinceName = segments[3].Trim(), // tỉnh thành
                ClientOrderCode = order.Id.ToString(),
                CODAmount = order.PaymentMethod!.Equals(PaymentMethods.VNPay) ||order.PaymentMethod!.Equals(PaymentMethods.ZaloPay) ? 0 : order.TotalAmount,
                Content = model.Content,
                Weight = model.Weight, //gram
                Length = model.Length, //cm
                Width = model.Width, //cm
                Height = model.Height, //cm
                InsuranceValue = order.TotalAmount,
                Items = items
            };
            var resutl = await GhnHelper.CreateShippingOrderAsync(requestData);
            if (resutl != null)
            {
                order.Status = OrderStatus.Transport;
                order.ShippingCode = resutl.OrderCode;
                _orderRepository.Update(order);
                var save = await _unitOfWork.SaveChanges();

                return save > 0 ? resutl : throw new BadRequestException("Lỗi khi update trạng thái đơn");

            }
            throw new BadRequestException("Vui lòng kiểm tra lại các thông tin như SĐT, địa chỉ");
        }


        public async Task GhnCallBack(GhnWebHookResponse model)
        {
            if (model.Status!.Equals("delivered"))
            {
                var order = await _orderRepository.GetMany(order => order.ShippingCode != null && order.ShippingCode.Equals(model.OrderCode)).FirstOrDefaultAsync();
                order!.Status = OrderStatus.Finished;
                _orderRepository.Update(order);
                await _unitOfWork.SaveChanges();
                await SendNotificationToCustomer(order);
            }
        }


        private async Task SendNotificationToCustomer(Order order)
        {
            var message = new CreateNotificationModel
            {
                Title = $"Giao hàng thành công.",
                Body = $"Đơn hàng {order.Id} của bạn được giao thành công. Cảm ơn bạn đã sử dụng dịch vụ bên chúng tôi.",
                Data = new NotificationDataViewModel
                {
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    Type = NotificationType.RepairService.ToString(),
                    Link = order.Id
                }
            };
            await _notificationService.SendNotification(new List<Guid> { (Guid)order.CustomerId! }, message);
        }
    }
}
