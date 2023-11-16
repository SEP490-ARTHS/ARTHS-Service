using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Implementations;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Enums;
using ARTHS_Utility.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class OrderService : BaseService, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRepairBookingRepository _repairBookingRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMotobikeProductRepository _motobikeProductRepository;
        private readonly IRepairServiceRepository _repairServiceRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly INotificationService _notificationService;
        private readonly IConfigurationService _configurationService;
        private readonly IRevenueStoreRepository _revenueStoreRepository;
        private readonly IMaintenanceScheduleRepository _maintenanceScheduleRepository;
        private readonly IWarrantyHistoryRepository _warrantyHistoryRepository;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService, IConfigurationService configurationService) : base(unitOfWork, mapper)
        {
            _orderRepository = unitOfWork.Order;
            _repairBookingRepository = unitOfWork.RepairBooking;
            _orderDetailRepository = unitOfWork.OrderDetail;
            _motobikeProductRepository = unitOfWork.MotobikeProduct;
            _repairServiceRepository = unitOfWork.RepairService;
            _accountRepository = unitOfWork.Account;
            _cartRepository = unitOfWork.Cart;
            _cartItemRepository = unitOfWork.CartItem;
            _notificationService = notificationService;
            _configurationService = configurationService;
            _revenueStoreRepository = unitOfWork.RevenueStore;
            _maintenanceScheduleRepository = unitOfWork.MaintenanceSchedule;
            _warrantyHistoryRepository = unitOfWork.WarrantyHistory;
        }

        public async Task<ListViewModel<OrderViewModel>> GetOrders(OrderFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _orderRepository.GetAll().AsQueryable();

            if (filter.StaffId.HasValue)
            {
                query = query.Where(order => order.StaffId.Equals(filter.StaffId));
            }
            if (filter.CustomerId.HasValue)
            {
                query = query.Where(order => order.CustomerId.Equals(filter.StaffId));
            }
            if (!string.IsNullOrEmpty(filter.OrderId))
            {
                query = query.Where(order => order.Id.Contains(filter.OrderId));
            }
            if (!string.IsNullOrEmpty(filter.CustomerName))
            {
                query = query.Where(order => (order.CustomerName != null && order.CustomerName.Contains(filter.CustomerName)));
            }
            if (!string.IsNullOrEmpty(filter.CustomerPhone))
            {
                query = query.Where(order => order.CustomerPhoneNumber.Contains(filter.CustomerPhone));
            }
            if (!string.IsNullOrEmpty(filter.OrderStatus))
            {
                query = query.Where(order => order.Status.Equals(filter.OrderStatus));
            }
            if (!string.IsNullOrEmpty(filter.ExcludeOrderStatus))
            {
                query = query.Where(order => !order.Status.Equals(filter.ExcludeOrderStatus));
            }
            if (filter.OrderType.HasValue)
            {
                query = query.Where(order => order.OrderType.Equals(filter.OrderType.ToString()));
            }


            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
                .OrderByDescending(order => order.OrderDate)
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);
            var orders = await paginatedQuery
                .ProjectTo<OrderViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            return new ListViewModel<OrderViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = orders
            };
        }

        public async Task<OrderViewModel> GetOrder(string Id)
        {
            return await _orderRepository.GetMany(order => order.Id.Equals(Id))
                .ProjectTo<OrderViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy order");
        }

        public async Task<OrderViewModel> CreateOrderOnline(Guid customerId, CreateOrderOnlineModel model)
        {
            var result = 0;
            var orderId = string.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    orderId = GenerateOrderId();
                    var orderType = OrderType.Online.ToString();
                    int totalPrice = await CreateOrderOnlineDetail(orderId, model.OrderDetailModels);
                    int shippingMoney = 0;
                    if (totalPrice <= 1000000)
                    {
                        var config = await _configurationService.GetSetting();
                        shippingMoney = config.ShippingMoney;
                        totalPrice += shippingMoney;
                    }
                    var customerName = await _accountRepository.GetMany(cus => cus.Id.Equals(customerId))
                        .Select(acc => acc.CustomerAccount!.FullName)
                        .FirstOrDefaultAsync();
                    var order = new Order
                    {
                        Id = orderId,
                        CustomerId = customerId,
                        CustomerName = customerName,
                        CustomerPhoneNumber = model.CustomerPhoneNumber,
                        Address = model.Address,
                        PaymentMethod = model.PaymentMethod,
                        Status = OrderStatus.Processing,
                        OrderType = orderType,
                        ShippingMoney = shippingMoney,
                        TotalAmount = totalPrice
                    };
                    _orderRepository.Add(order);

                    await RemoveProductFromCart(customerId, model.OrderDetailModels);

                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetOrder(orderId) : null!;
        }


        public async Task<OrderViewModel> UpdateOrderOnline(string Id, UpdateOrderOnlineModel model)
        {
            var order = await _orderRepository.GetMany(order => order.Id.Equals(Id)).FirstOrDefaultAsync();
            if (order == null)
            {
                throw new NotFoundException("Không tìm thấy order");
            }
            if (model.Status.Equals(OrderStatus.Canceled) && order.Status != OrderStatus.Processing)
            {
                throw new ConflictException("Không thể hủy đơn hàng này");
            }
            if (model.Status == OrderStatus.Canceled && string.IsNullOrEmpty(model.CancellationReason))
            {
                throw new BadRequestException("Cần phải cung cấp lý do khi hủy đơn hàng.");
            }
            if (model.Status.Equals(OrderStatus.Canceled))
            {
                order.CancellationReason = model.CancellationReason;
                order.CancellationDate = DateTime.UtcNow;
            }

            order.Status = model.Status ?? order.Status;

            if (ShouldCreateTransaction(order.PaymentMethod!, order.Status))
            {
                await CreateTransaction(order);
            }

            _orderRepository.Update(order);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetOrder(Id) : null!;
        }


        public async Task<OrderViewModel> CreateOrderOffline(Guid tellerId, CreateOrderOfflineModel model)
        {
            var result = 0;
            
            var orderId = string.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    bool sendNotifyToStaff = false;
                    orderId = GenerateOrderId();
                    int totalPrice = await CreateOrderOfflineDetail(orderId, model.OrderDetailModel, false);
                    var orderType = OrderType.Offline.ToString();

                    var order = new Order
                    {
                        Id = orderId,
                        //StaffId = model.StaffId ?? null,
                        TellerId = tellerId,
                        CustomerPhoneNumber = model.CustomerPhoneNumber,
                        CustomerName = model.CustomerName,
                        LicensePlate = model.LicensePlate,
                        OrderType = orderType,
                        Status = OrderStatus.Processing,
                        TotalAmount = totalPrice
                    };
                    _orderRepository.Add(order);

                    //add staff or not
                    if (ShouldAddStaffToOrder(model.OrderDetailModel))
                    {
                        if (!model.StaffId.HasValue) throw new BadRequestException("Vui lòng chọn nhân viên");
                        order.StaffId = model.StaffId;

                        //check booking
                        if (model.BookingId.HasValue)
                        {
                            var booking = await _repairBookingRepository.GetMany(booking => booking.Id.Equals(model.BookingId.Value))
                                .FirstOrDefaultAsync();
                            if (booking == null) throw new NotFoundException($"Không tìm thấy booking {model.BookingId}");
                            booking.OrderId = orderId;
                            _repairBookingRepository.Update(booking);

                            order.CustomerId = booking.CustomerId;
                        }

                        sendNotifyToStaff = true;
                    }

                    result = await _unitOfWork.SaveChanges();
                    if (sendNotifyToStaff)
                    {
                        await SendNotificationToStaff(order);
                    }
                    transaction.Commit();

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetOrder(orderId) : null!;
        }

        public async Task<OrderViewModel> UpdateOrderOffline(string Id, UpdateInStoreOrderModel model)
        {
            var order = await _orderRepository.GetMany(order => order.Id.Equals(Id)).FirstOrDefaultAsync();
            if (order == null)
            {
                throw new NotFoundException("Không tìm thấy order.");
            }
            if (order.Status.Equals(OrderStatus.Finished))
            {
                throw new BadRequestException("Đơn đã hoàn thành không thể chỉnh sữa");
            }
            if (model.Status != null)
            {
                if (model.Status.Equals(OrderStatus.Finished))
                {
                    await CreateTransaction(order);
                    await CreateMaintenanceSchedule(order.Id);
                }

                if (model.Status.Equals(OrderStatus.WaitForPay))
                {
                    await SendNotificationToTellers(order);
                }
                order.Status = model.Status;
            }

            order.StaffId = model.StaffId ?? order.StaffId;
            order.CustomerName = model.CustomerName ?? order.CustomerName;
            order.CustomerPhoneNumber = model.CustomerPhone ?? order.CustomerPhoneNumber;
            order.LicensePlate = model.LicensePlate ?? order.LicensePlate;
            if (model.OrderDetailModel != null && model.OrderDetailModel.Count > 0)
            {
                order.TotalAmount = await CreateOrderOfflineDetail(Id, model.OrderDetailModel, true);
                if (!ShouldAddStaffToOrder(model.OrderDetailModel))
                {
                    order.StaffId = null;
                }
            }

            _orderRepository.Update(order);
            return await _unitOfWork.SaveChanges() > 0 ? await GetOrder(Id) : null!;

        }


        

        //PRIVATE
        private async Task SendNotificationToTellers(Order order)
        {
            var message = new CreateNotificationModel
            {
                Title = $"Đơn sửa chữa của khách hàng {order.CustomerName} đã hoàn thành",
                Body = "Đơn sửa chữa của khách hàng đã được sửa xong. Vui lòng tiến hành thanh toán.",
                Data = new NotificationDataViewModel
                {
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    Type = NotificationType.RepairService.ToString(),
                    Link = order.Id
                }
            };
            var tellers = await _accountRepository.GetMany(acc => acc.Role.RoleName.Equals(UserRole.Teller))
                .Include(acc => acc.Role)
                .Select(acc => acc.Id)
                .ToListAsync();
            await _notificationService.SendNotification(tellers, message);
        }
        private async Task CreateTransaction(Order order)
        {
            var revenue = new RevenueStore
            {
                Id = DateTime.UtcNow.AddHours(7).ToString("yyMMdd") + "_" + order.Id,
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Type = "Thanh toán hóa đơn mua hàng online của cửa hàng Thanh Huy",
                PaymentMethod = PaymentMethods.COD,
                Status = "Thành công"
            };

            _revenueStoreRepository.Add(revenue);
            await _unitOfWork.SaveChanges();
        }

        private bool ShouldCreateTransaction(string paymentMethod, string status)
        {
            if (paymentMethod == PaymentMethods.COD && status == OrderStatus.Finished)
            {
                return true;
            }
            return false;
        }

        private async Task RemoveProductFromCart(Guid customerId, List<CreateOrderOnlineDetailModel> listDetail)
        {
            var productRemoveIds = listDetail.Select(list => list.MotobikeProductId).ToList();

            var listProductInCart = await _cartRepository.GetMany(cart => cart.CustomerId.Equals(customerId)).Include(cart => cart.CartItems).FirstOrDefaultAsync();
            var productToRemove = listProductInCart!.CartItems.Where(cart => productRemoveIds.Contains(cart.MotobikeProductId));

            _cartItemRepository.RemoveRange(productToRemove);
        }

        private bool ShouldAddStaffToOrder(List<CreateOrderOfflineDetailModel> listDetail)
        {
            if (listDetail.Any(detail => detail.RepairServiceId.HasValue) || listDetail.Any(detail => detail.InstUsed.Equals(true)))
            {
                return true;
            }
            return false;
        }

        private async Task<int> CreateOrderOfflineDetail(string orderId, List<CreateOrderOfflineDetailModel> listDetail, bool isUpdate)
        {
            if (listDetail.Count == 0)
            {
                throw new BadRequestException("Phải có ý nhất một sản phẩm hoặc dịch vụ để order");
            }
            if (isUpdate)
            {
                var existOrderDetail = await _orderDetailRepository.GetMany(detail => detail.OrderId.Equals(orderId)).ToListAsync();
                _orderDetailRepository.RemoveRange(existOrderDetail);
            }
            int totalPrice = 0;
            foreach (var detail in listDetail)
            {
                if (detail.MotobikeProductId.HasValue && detail.RepairServiceId.HasValue)
                {
                    throw new BadRequestException("Mỗi detail chỉ chứa product hoặc repair service");
                }
                int detailPrice = 0;
                var orderDetail = new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    WarrantyStartDate = DateTime.UtcNow.AddHours(7)
                };

                if (detail.MotobikeProductId.HasValue)
                {
                    var product = await _motobikeProductRepository.GetMany(product => product.Id.Equals(detail.MotobikeProductId))
                        .Include(product => product.Discount)
                        .Include(product => product.Warranty)
                        .FirstOrDefaultAsync();
                    if (product == null) throw new NotFoundException($"Không tìm thấy sản phẩm {detail.MotobikeProductId}");

                    //áp dụng giảm giá
                    int productPrice = product.PriceCurrent;
                    if (product.Discount != null)
                    {
                        productPrice = productPrice * (100 - product.Discount.DiscountAmount) / 100;
                    }
                    orderDetail.Price = productPrice;
                    detailPrice = productPrice * detail.ProductQuantity.GetValueOrDefault(1);
                    orderDetail.MotobikeProductId = detail.MotobikeProductId;
                    orderDetail.Quantity = detail.ProductQuantity.GetValueOrDefault(1);

                    //tính phí thay thế + bảo hảnh
                    var warrantyEndDate = DateTime.UtcNow.AddHours(7);
                    if (detail.InstUsed.GetValueOrDefault(false))
                    {
                        orderDetail.InstUsed = detail.InstUsed.GetValueOrDefault(true);
                        detailPrice += product.InstallationFee;
                        warrantyEndDate = product.Warranty != null ? DateTime.UtcNow.AddMonths(product.Warranty.Duration) : warrantyEndDate;
                    }

                    orderDetail.WarrantyEndDate = warrantyEndDate;
                }
                else if (detail.RepairServiceId.HasValue)
                {
                    var repairService = await _repairServiceRepository.GetMany(service => service.Id.Equals(detail.RepairServiceId)).FirstOrDefaultAsync();
                    if (repairService == null) throw new NotFoundException($"Không tìm thấy dịch vụ {detail.RepairServiceId}");
                    
                    int servicePrice = repairService.Price;
                    if(repairService.Discount != null)
                    {
                        servicePrice = servicePrice * (100 - repairService.Discount.DiscountAmount) / 100;
                    }
                    detailPrice = servicePrice;
                    orderDetail.RepairServiceId = detail.RepairServiceId;
                    orderDetail.Price = servicePrice;

                    orderDetail.WarrantyEndDate = repairService.WarrantyDuration == 0 ? null : DateTime.UtcNow.AddHours(7).AddMonths(repairService.WarrantyDuration);
                    
                    //if (repairService.ReminderInterval.HasValue)
                    //{
                    //    CreateMaintenanceSchedule(orderDetail.Id, (int)repairService.ReminderInterval);
                    //}
                }

                orderDetail.SubTotalAmount = detailPrice;
                _orderDetailRepository.Add(orderDetail);

                totalPrice += detailPrice;
            }
            return totalPrice;
        }

        private async Task CreateMaintenanceSchedule(string orderId)
        {
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    var order = await _orderRepository.GetMany(order => order.Id.Equals(orderId) && order.CustomerId != null)
                        .Include(order => order.OrderDetails)
                            .ThenInclude(detail => detail.RepairService)
                        .FirstOrDefaultAsync();
                    if (order == null) return;
                    var details = order.OrderDetails.Where(detail => detail.RepairService != null && detail.RepairService.ReminderInterval != null);
                    if (details == null) return;

                    foreach (var detail in details)
                    {
                        var nextMaintenanceDate = DateTime.UtcNow.AddMonths((int)detail.RepairService!.ReminderInterval!);
                        var reminderDate = nextMaintenanceDate.AddDays(-15);
                        var schedule = new MaintenanceSchedule
                        {
                            Id = Guid.NewGuid(),
                            CustomerId = (Guid)order.CustomerId!,
                            OrderDetailId = detail.Id,
                            NextMaintenanceDate = nextMaintenanceDate,
                            ReminderDate = reminderDate,
                        };
                        _maintenanceScheduleRepository.Add(schedule);
                    }
                    await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
        }

        private async Task<int> CreateOrderOnlineDetail(string orderId, List<CreateOrderOnlineDetailModel> listDetail)
        {
            if (listDetail.Count == 0)
            {
                throw new BadRequestException("Phải có ý nhất một sản phẩm hoặc dịch vụ để order");
            }
            int totalPrice = 0;
            foreach (var detail in listDetail)
            {
                int detailPrice = 0;
                var orderDetail = new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    WarrantyStartDate = DateTime.UtcNow.AddHours(7),
                    WarrantyEndDate = DateTime.UtcNow.AddHours(7)
                };

                var product = await _motobikeProductRepository.GetMany(product => product.Id.Equals(detail.MotobikeProductId))
                    .Include(product => product.Discount)
                    .Include(product => product.Warranty)
                    .FirstOrDefaultAsync();
                if (product == null) throw new NotFoundException($"Không tìm thấy sản phẩm {detail.MotobikeProductId}");

                //áp dụng giảm giá
                int productPrice = product.PriceCurrent;
                if (product.Discount != null)
                {
                    productPrice = productPrice * (100 - product.Discount.DiscountAmount) / 100;
                }
                orderDetail.Price = productPrice;
                detailPrice = productPrice * detail.ProductQuantity;
                orderDetail.MotobikeProductId = detail.MotobikeProductId;
                orderDetail.Quantity = detail.ProductQuantity;
                orderDetail.SubTotalAmount = detailPrice;

                _orderDetailRepository.Add(orderDetail);

                totalPrice += detailPrice;
            }
            return totalPrice;
        }

        private static string GenerateOrderId()
        {
            long ticks = DateTime.UtcNow.Ticks;
            int hash = HashCode.Combine(ticks);
            uint positiveHash = (uint)hash & 0x7FFFFFFF;
            string hashString = positiveHash.ToString("X8");
            string id = "OR" + hashString;

            return id;
        }

        private async Task SendNotificationToStaff(Order order)
        {
            var message = new CreateNotificationModel
            {
                Title = $"Đơn sửa chữa của khách hàng {order.CustomerName}.",
                Body = $"Đơn hàng {order.Id} đã được bàn giao cho bạn. Vui lòng tiến hành sửa chữa.",
                Data = new NotificationDataViewModel
                {
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    Type = NotificationType.RepairService.ToString(),
                    Link = order.Id
                }
            };
            //var staffId = await _accountRepository.GetMany(account => account.Id.Equals(order.StaffId)).Select(account => account.Id).FirstOrDefaultAsync();
            await _notificationService.SendNotification(new List<Guid> { (Guid)order.StaffId! }, message);
        }

    }
}
