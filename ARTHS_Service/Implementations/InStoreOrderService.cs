﻿using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
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

        public InStoreOrderService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _inStoreOrderRepository = unitOfWork.InStoreOrder;
            _inStoreOrderDetailRepository = unitOfWork.InStoreOrderDetail;
            _motobikeProductRepository = unitOfWork.MotobikeProduct;
            _repairServiceRepository = unitOfWork.RepairService;
        }
        public async Task<List<BasicInStoreOrderViewModel>> GetInStoreOrders(InStoreOrderFilterModel filter)
        {
            var query = _inStoreOrderRepository.GetAll();

            if (filter.Id != null)
            {
                query = query.Where(order => order.Id.Contains(filter.Id));
            }
            if (filter.CustomerName != null)
            {
                query = query.Where(order => (order.CustomerName != null && order.CustomerName.Contains(filter.CustomerName)));
            }
            if (filter.CustomerPhone != null)
            {
                query = query.Where(order => order.CustomerPhone.Contains(filter.CustomerPhone));
            }

            return await query
                .ProjectTo<BasicInStoreOrderViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

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
            inStoreOrder.Status = model.Status ?? inStoreOrder.Status;

            if (model.OrderDetailModel != null && model.OrderDetailModel.Count > 0)
            {
                inStoreOrder.TotalAmount = await HandleInStoreOrderDetail(orderId, model.OrderDetailModel, false);
                inStoreOrder.OrderType = StoreOrderType(model.OrderDetailModel);
            }

            _inStoreOrderRepository.Update(inStoreOrder);
            return await _unitOfWork.SaveChanges() > 0 ? await GetInStoreOrder(orderId) : null!;
        }


        //PRIVATE METHOD
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
                (int totalProductPrice, DateTime warrantyPeriod) = await GetProductPriceAndWarranty(detail.MotobikeProductId, detail.ProductQuantity);
                int repairServicePrice = await GetRepairServicePrice(detail.RepairServiceId);

                totalAmount += totalProductPrice + repairServicePrice;

                var orderDetail = new InStoreOrderDetail
                {
                    Id = Guid.NewGuid(),
                    InStoreOrderId = orderId,
                    RepairServiceId = detail.RepairServiceId,
                    MotobikeProductId = detail.MotobikeProductId,
                    ProductQuantity = detail.MotobikeProductId != null ? detail.ProductQuantity : (int?)null,
                    ProductPrice = totalProductPrice,
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
        private async Task<(int totalProductPrice, DateTime warrantyPeriod)> GetProductPriceAndWarranty(Guid? productId, int? quantity)
        {
            if (!productId.HasValue)
            {
                return (0, DateTime.UtcNow);
            }

            var product = await _motobikeProductRepository.GetMany(p => p.Id.Equals(productId))
                                                          .Include(p => p.Warranty)
                                                          .FirstOrDefaultAsync();
            if(product == null)
            {
                throw new NotFoundException("Không tìm thấy product.");
            }

            int price = product.PriceCurrent;
            int actualQuantity = (quantity ?? 0) <= 0 ? 1 : quantity!.Value;
            int totalProductPrice = price * actualQuantity;

            DateTime warrantyPeriod = product?.Warranty?.Duration != null
                ? DateTime.UtcNow.AddMonths(product.Warranty.Duration)
                : DateTime.UtcNow;

            return (totalProductPrice, warrantyPeriod);
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
            if(service == null)
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
            if(details.All(detail => !detail.RepairServiceId.HasValue))
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