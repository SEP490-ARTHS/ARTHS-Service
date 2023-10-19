using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Implementations;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class OnlineOrderService : BaseService, IOnlineOrderService
    {
        private readonly IOnlineOrderRepository _onlineOrderRepository;
        private readonly IOnlineOrderDetailRepository _onlineOrderDetailRepository;
        private readonly IMotobikeProductRepository _motobikeProductRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ITransactionRepository _transactionRepository;

        public OnlineOrderService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _onlineOrderRepository = unitOfWork.OnlineOrder;
            _onlineOrderDetailRepository = unitOfWork.OnlineOrderDetail;
            _motobikeProductRepository = unitOfWork.MotobikeProduct;
            _cartRepository = unitOfWork.Cart;
            _cartItemRepository = unitOfWork.CartItem;
            _transactionRepository = unitOfWork.Transactions;
        }

        public async Task<ListViewModel<OnlineOrderViewModel>> GetOrders(PaginationRequestModel pagination)
        {
            var query = _onlineOrderRepository.GetAll();

            var listOrder = query
                .ProjectTo<OnlineOrderViewModel>(_mapper.ConfigurationProvider)
                .OrderByDescending(order => order.OrderDate);
            var orders = await listOrder.Skip(pagination.PageNumber * pagination.PageSize).Take(pagination.PageSize).AsNoTracking().ToListAsync();
            var totalRow = await listOrder.AsNoTracking().CountAsync();

            if(orders != null || orders != null && orders.Any())
            {
                return new ListViewModel<OnlineOrderViewModel>
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

        public async Task<OnlineOrderViewModel> GetOrder(Guid id)
        {
            return await _onlineOrderRepository.GetMany(order => order.Id.Equals(id))
                .ProjectTo<OnlineOrderViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy order");
        }

        public async Task<List<OnlineOrderViewModel>> GetOrdersByCustomerId(Guid customerId)
        {
            return await _onlineOrderRepository.GetMany(order => order.CustomerId.Equals(customerId))
                .ProjectTo<OnlineOrderViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<OnlineOrderViewModel> CreateOrder(Guid cartId, CreateOnlineOrderModel model)
        {
            var cart = await _cartRepository.GetMany(cart => cart.Id.Equals(cartId))
                .Include(cart => cart.CartItems)
                    .ThenInclude(item => item.MotobikeProduct)
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                throw new NotFoundException("Không tìm thấy cart");
            }
            if (cart.CartItems.Count == 0)
            {
                throw new BadRequestException("Phải có ý nhất một sản phẩm trong giỏ hàng để order");
            }

            var result = 0;
            var orderId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    orderId = Guid.NewGuid();
                    int totalAmount = await HandleCreateOrderDetail(orderId, cart.CartItems);

                    var order = new OnlineOrder
                    {
                        Id = orderId,
                        CustomerId = cart.CustomerId,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        PaymentMethod = model.PaymentMethod,
                        Status = OnlineOrderStatus.Processing,
                        TotalAmount = totalAmount,
                    };
                    _onlineOrderRepository.Add(order);

                    //Remove item in cart after order
                    _cartItemRepository.RemoveRange(cart.CartItems);


                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return result > 0 ? await GetOrder(orderId) : null!;
        }

        public async Task<OnlineOrderViewModel> UpdateOrder(Guid id, UpdateOnlineOrderModel model)
        {
            var order = await _onlineOrderRepository.GetMany(order => order.Id.Equals(id)).FirstOrDefaultAsync();
            if(order == null)
            {
                throw new NotFoundException("Không tìm thấy order");
            }
            if(model.Status.Equals(OnlineOrderStatus.Canceled) && order.Status != OnlineOrderStatus.Processing)
            {
                throw new ConflictException("Không thể hủy đơn hàng này");
            }
            if (model.Status == OnlineOrderStatus.Canceled && string.IsNullOrEmpty(model.CancellationReason))
            {
                throw new BadRequestException("Cần phải cung cấp lý do khi hủy đơn hàng.");
            }
            if (model.Status.Equals(OnlineOrderStatus.Canceled))
            {
                order.CancellationReason = model.CancellationReason;
                order.CancellationDate = DateTime.UtcNow;
            }

            order.Status = model.Status ?? order.Status;

            if(ShouldCreateTransaction(order.PaymentMethod, order.Status))
            {
                await CreateTransaction(order);
            }

            _onlineOrderRepository.Update(order);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetOrder(id) : null!;

        }

        //PRIVATE METHOD
        private async Task CreateTransaction(OnlineOrder onlineOrder)
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                OnlineOrderId = onlineOrder.Id,
                TotalAmount = onlineOrder.TotalAmount,
                Type = "Thanh toán hóa đơn mua hàng online của cửa hàng Thanh Huy",
                PaymentMethod = "Tiền mặt",
                Status = "Thành công"
            };

            _transactionRepository.Add(transaction);
            await _unitOfWork.SaveChanges();
        }

        private bool ShouldCreateTransaction(string paymentMethod, string status)
        {
            if (paymentMethod == PaymentMethods.COD && status == OnlineOrderStatus.Finished)
            {
                return true;
            }
            return false;
        }

        private async Task<int> HandleCreateOrderDetail(Guid orderId, ICollection<CartItem> items)
        {
            var totalAmount = 0;
            var listOrderDetail = new List<OnlineOrderDetail>();
            foreach (var item in items)
            {
                var product = await _motobikeProductRepository.GetMany(product => product.Id.Equals(item.MotobikeProductId)).FirstOrDefaultAsync();
                if (product != null)
                {
                    var orderDetail = new OnlineOrderDetail
                    {
                        OnlineOrderId = orderId,
                        MotobikeProductId = product.Id,
                        Price = product.PriceCurrent,
                        Quantity = item.Quantity,
                        SubTotalAmount = product.PriceCurrent * item.Quantity,
                    };
                    listOrderDetail.Add(orderDetail);

                    totalAmount += orderDetail.SubTotalAmount;
                }
            }
            _onlineOrderDetailRepository.AddRange(listOrderDetail);
            return totalAmount;
        }

    }
}
