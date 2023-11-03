using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
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
    public class InStoreOrderService : BaseService, IInStoreOrderService
    {
        private readonly IInStoreOrderRepository _inStoreOrderRepository;
        private readonly IInStoreOrderDetailRepository _inStoreOrderDetailRepository;
        private readonly IMotobikeProductRepository _motobikeProductRepository;
        private readonly IRepairServiceRepository _repairServiceRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;

        public InStoreOrderService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService) : base(unitOfWork, mapper)
        {
            _inStoreOrderRepository = unitOfWork.InStoreOrder;
            _inStoreOrderDetailRepository = unitOfWork.InStoreOrderDetail;
            _motobikeProductRepository = unitOfWork.MotobikeProduct;
            _repairServiceRepository = unitOfWork.RepairService;
            _transactionRepository = unitOfWork.Transactions;
            _accountRepository = unitOfWork.Account;
            _notificationService = notificationService;
        }
        public async Task<ListViewModel<BasicInStoreOrderViewModel>> GetInStoreOrders(InStoreOrderFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _inStoreOrderRepository.GetAll();

            if (filter.StaffId.HasValue)
            {
                query = query.Where(order => order.StaffId.Equals(filter.StaffId));
            }
            if (filter.CustomerName != null)
            {
                query = query.Where(order => (order.CustomerName != null && order.CustomerName.Contains(filter.CustomerName)));
            }
            if (filter.CustomerPhone != null)
            {
                query = query.Where(order => order.CustomerPhone.Contains(filter.CustomerPhone));
            }
            if (filter.OrderStatus != null)
            {
                query = query.Where(order => order.Status.Equals(filter.OrderStatus));
            }
            if (filter.ExcludeOrderStatus != null)
            {
                query = query.Where(order => !order.Status.Equals(filter.ExcludeOrderStatus));
            }
            var listOrder = query
                .ProjectTo<BasicInStoreOrderViewModel>(_mapper.ConfigurationProvider)
                .OrderByDescending(order => order.OrderDate);

            var orders = await listOrder.Skip(pagination.PageNumber * pagination.PageSize).Take(pagination.PageSize).AsNoTracking().ToListAsync();
            var totalRow = await listOrder.AsNoTracking().CountAsync();
            if (orders != null || orders != null && orders.Any())
            {
                return new ListViewModel<BasicInStoreOrderViewModel>
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
            return null!;
        }

        public async Task<InStoreOrderViewModel> GetInStoreOrder(string id)
        {
            return await _inStoreOrderRepository.GetMany(order => order.Id.Equals(id))
                .ProjectTo<InStoreOrderViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy order");
        }

        public async Task<InStoreOrderViewModel> CreateInStoreOrder(Guid tellerId, CreateInStoreOrderModel model)
        {
            var result = 0;
            var inStoreOrderId = string.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    inStoreOrderId = GenerateOrderId();
                    var totalPrice = await HandleInStoreOrderDetail(inStoreOrderId, model.OrderDetailModel, true);
                    var orderType = StoreOrderType(model.OrderDetailModel);
                    var inStoreOrder = new InStoreOrder
                    {
                        Id = inStoreOrderId,
                        TellerId = tellerId,
                        StaffId = model.StaffId,
                        CustomerName = model.CustomerName,
                        CustomerPhone = model.CustomerPhone,
                        LicensePlate = model.LicensePlate,
                        Status = InStoreOrderStatus.NewOrder,
                        OrderType = orderType,
                        TotalAmount = totalPrice
                    };
                    _inStoreOrderRepository.Add(inStoreOrder);

                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetInStoreOrder(inStoreOrderId) : null!;
        }

        public async Task<InStoreOrderViewModel> UpdateInStoreOrder(string orderId, UpdateInStoreOrderModel model)
        {
            var inStoreOrder = await _inStoreOrderRepository.GetMany(order => order.Id.Equals(orderId)).FirstOrDefaultAsync();
            if (inStoreOrder == null)
            {
                throw new NotFoundException("Không tìm thấy order.");
            }
            if (inStoreOrder.Status.Equals(InStoreOrderStatus.Finished))
            {
                throw new BadRequestException("Đơn đã hoàn thành không thể chỉnh sữa");
            }


            inStoreOrder.StaffId = model.StaffId ?? inStoreOrder.StaffId;
            inStoreOrder.CustomerName = model.CustomerName ?? inStoreOrder.CustomerName;
            inStoreOrder.CustomerPhone = model.CustomerPhone ?? inStoreOrder.CustomerPhone;
            inStoreOrder.LicensePlate = model.LicensePlate ?? inStoreOrder.LicensePlate;


            if (model.Status != null)
            {
                //khi nào status là paid thì mới tạo transaction
                if (model.Status.Equals(InStoreOrderStatus.Paid) && !inStoreOrder.Status.Equals(InStoreOrderStatus.Paid))
                {

                    await CreateTransaction(inStoreOrder);
                }
                if (model.Status.Equals(InStoreOrderStatus.WaitForPay))
                {
                    await SendNotification(inStoreOrder);
                }
                inStoreOrder.Status = model.Status;
            }

            if (model.OrderDetailModel != null && model.OrderDetailModel.Count > 0)
            {
                inStoreOrder.TotalAmount = await HandleInStoreOrderDetail(orderId, model.OrderDetailModel, false);
                inStoreOrder.OrderType = StoreOrderType(model.OrderDetailModel);
            }

            _inStoreOrderRepository.Update(inStoreOrder);
            return await _unitOfWork.SaveChanges() > 0 ? await GetInStoreOrder(orderId) : null!;
        }


        //PRIVATE METHOD
        private async Task CreateTransaction(InStoreOrder inStoreOrder)
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                InStoreOrderId = inStoreOrder.Id,
                TotalAmount = inStoreOrder.TotalAmount,
                Type = "Thanh toán đơn hàng tại cửa hàng Thanh Huy",
                PaymentMethod = "Tiền mặt",
                Status = "Thành công"
            };

            _transactionRepository.Add(transaction);
            await _unitOfWork.SaveChanges();
        }

        private async Task SendNotification(InStoreOrder order)
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

        /// <summary>
        /// Processes in-store order details and returns the total amount.
        /// </summary>
        /// <param name="orderId">The in-store order ID.</param>
        /// <param name="listDetails">Details for the order.</param>
        /// <param name="isNewOrder">Whether it's a new order or an update.</param>
        /// <returns>Total amount of the order.</returns>
        private async Task<int> HandleInStoreOrderDetail(string orderId, List<CreateInStoreOrderDetailModel> listDetails, bool isNewOrder)
        {
            int totalAmount = 0;
            var listOrderDetail = new List<InStoreOrderDetail>();

            if (!isNewOrder)
            {
                var existOrderDetails = await _inStoreOrderDetailRepository.GetMany(detail => detail.InStoreOrderId.Equals(orderId)).ToListAsync();
                _inStoreOrderDetailRepository.RemoveRange(existOrderDetails);
            }

            foreach (var detail in listDetails)
            {
                (int productPrice, int totalProductPrice, DateTime warrantyPeriod) = await GetProductPriceAndWarranty(detail.MotobikeProductId, detail.ProductQuantity);
                int repairServicePrice = await GetRepairServicePrice(detail.RepairServiceId);

                totalAmount += totalProductPrice + repairServicePrice;

                var orderDetail = new InStoreOrderDetail
                {
                    Id = Guid.NewGuid(),
                    InStoreOrderId = orderId,
                    RepairServiceId = detail.RepairServiceId,
                    MotobikeProductId = detail.MotobikeProductId,
                    ProductQuantity = detail.MotobikeProductId != null ? detail.ProductQuantity : (int?)null,
                    ProductPrice = productPrice,
                    ServicePrice = repairServicePrice,
                    WarrantyPeriod = warrantyPeriod
                };

                listOrderDetail.Add(orderDetail);
            }
            _inStoreOrderDetailRepository.AddRange(listOrderDetail);
            return totalAmount;
        }


        /// <summary>
        /// Retrieves the total product price and warranty period for a given product.
        /// </summary>
        /// <param name="productId">ID of the product.</param>
        /// <param name="quantity">Quantity of the product.</param>
        /// <returns>Total product price and warranty expiration date.</returns>
        private async Task<(int productPrice, int totalProductPrice, DateTime warrantyPeriod)> GetProductPriceAndWarranty(Guid? productId, int? quantity)
        {
            if (!productId.HasValue)
            {
                return (0, 0, DateTime.UtcNow);
            }

            var product = await _motobikeProductRepository.GetMany(p => p.Id.Equals(productId))
                                                          .Include(p => p.Discount)
                                                          .Include(p => p.Warranty)
                                                          .FirstOrDefaultAsync();
            if (product == null)
            {
                throw new NotFoundException("Không tìm thấy product.");
            }

            int price = product.PriceCurrent;
            if (product.Discount != null)
            {
                price = price * (100 - product.Discount.DiscountAmount) / 100;
            }
            int actualQuantity = (quantity ?? 0) <= 0 ? 1 : quantity!.Value;
            int totalProductPrice = price * actualQuantity;

            DateTime warrantyPeriod = product?.Warranty?.Duration != null
                ? DateTime.UtcNow.AddMonths(product.Warranty.Duration)
                : DateTime.UtcNow;

            return (price, totalProductPrice, warrantyPeriod);
        }

        /// <summary>
        /// Retrieves the price of a specific repair service.
        /// </summary>
        /// <param name="repairServiceId">ID of the repair service.</param>
        /// <returns>Price of the repair service.</returns>
        private async Task<int> GetRepairServicePrice(Guid? repairServiceId)
        {
            if (!repairServiceId.HasValue)
            {
                return 0;
            }

            var service = await _repairServiceRepository.GetMany(service => service.Id.Equals(repairServiceId)).FirstOrDefaultAsync();
            if (service == null)
            {
                throw new NotFoundException("Không tìm thấy repair service");
            }
            return service.Price;
        }

        /// <summary>
        /// Determines the type of order based on its details.
        /// </summary>
        /// <returns>Purchase order or Repair order.</returns>
        private string StoreOrderType(List<CreateInStoreOrderDetailModel> details)
        {
            if (details.All(detail => !detail.RepairServiceId.HasValue))
            {
                return InStoreOrderType.Purchase;
            }
            return InStoreOrderType.Repair;
        }

        private string GenerateOrderId()
        {
            DateTime now = DateTime.Now;
            // Lấy tổng số giây từ thời điểm đầu tiên của năm hiện tại tới thời điểm hiện tại
            int seconds = (int)(now - new DateTime(now.Year, 1, 1)).TotalSeconds;
            // Lấy phần dư khi chia cho 10^8 để giữ ID dưới 8 ký tự
            int uniqueNumber = seconds % 100000000;
            // Tạo ID dưới dạng "OR" + 8 ký tự số
            string id = "OR" + uniqueNumber.ToString("D8"); // D8 giúp đảm bảo có đủ 8 chữ số
            return id;
        }
    }
}
