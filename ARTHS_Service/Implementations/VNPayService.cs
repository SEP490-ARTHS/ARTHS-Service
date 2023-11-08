using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Exceptions;
using ARTHS_Utility.Helpers.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class VNPayService : BaseService, IVNPayService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRevenueStoreRepository _revenueStoreRepository;

        public VNPayService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _orderRepository = unitOfWork.Order;
            _revenueStoreRepository = unitOfWork.RevenueStore;
        }


        public async Task<bool> ProcessInStoreOrderPayment(string orderId, VnPayRequestModel model)
        {
            var order = await _orderRepository.GetMany(order => order.Id.Equals(orderId)).FirstOrDefaultAsync();
            if (order == null) throw new NotFoundException("Không tìm thấy thông tin order");
            if (order.Status.Equals(OrderStatus.Paid)) throw new BadRequestException("Đơn hàng này đã thanh toán thành công rồi");

            var revenue = new RevenueStore
            {
                OrderId = orderId,
                TotalAmount = model.Amount,
                Type = "Thanh toán hóa đơn tại cửa hàng Thanh Huy",
                PaymentMethod = "VNPay",
                Status = "Đang xử lý",
                Id = model.TxnRef
            };
            _revenueStoreRepository.Add(revenue);
            return await _unitOfWork.SaveChanges() > 0;
        }

        public async Task<bool> ConfirmOrderPayment(VnPayResponseModel model)
        {
            var revenue = await _revenueStoreRepository.GetMany(transaction => transaction.Id.Equals(model.TxnRef)).FirstOrDefaultAsync();
            if (revenue == null) throw new NotFoundException("Không tìm thấy thông tin của revenue");


            var order = await _orderRepository.GetMany(order => order.Id.Equals(revenue.OrderId)).FirstOrDefaultAsync();
            if (order == null) return false;

            if (model.ResponseCode == "00")
            {
                order.Status = OrderStatus.Paid;
                revenue.Status = "Thành công";
            }
            else
            {
                revenue.Status = "Thất bại";
            }
            _orderRepository.Update(order);

            revenue.UpdateAt = DateTime.UtcNow;
            _revenueStoreRepository.Update(revenue);

            return await _unitOfWork.SaveChanges() > 0;
        }


    }
}
