using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Post;
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
        private readonly IOnlineOrderRepository _onlineOrderRepository;
        private readonly IInStoreOrderRepository _inStoreOrderRepository;
        private readonly ITransactionRepository _transactionRepository;

        public VNPayService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _onlineOrderRepository = unitOfWork.OnlineOrder;
            _inStoreOrderRepository = unitOfWork.InStoreOrder;
            _transactionRepository = unitOfWork.Transactions;
        }


        public async Task<bool> ProcessOnlineOrderPayment(Guid onlineOrderId, VnPayRequestModel model)
        {
            var order = await _onlineOrderRepository.GetMany(order => order.Id.Equals(onlineOrderId)).FirstOrDefaultAsync();
            if (order == null) throw new NotFoundException("Không tìm thấy thông tin order");
            if(order.Status.Equals(OnlineOrderStatus.Paid)) throw new BadRequestException("Đơn hàng này đã thanh toán thành công rồi");

            var transaction = new Transaction
            {
                OnlineOrderId = onlineOrderId,
                TotalAmount = model.Amount,
                Type = "Thanh toán hóa đơn mua hàng online của cửa hàng Thanh Huy",
                PaymentMethod = "VNPay",
                Status = "Đang xử lý",
                Id = model.TxnRef
            };
            _transactionRepository.Add(transaction);
            return await _unitOfWork.SaveChanges() > 0;
        }


        public async Task<bool> ProcessInStoreOrderPayment(string inStoreOrderId, VnPayRequestModel model)
        {
            var order = await _inStoreOrderRepository.GetMany(order => order.Id.Equals(inStoreOrderId)).FirstOrDefaultAsync();
            if (order == null) throw new NotFoundException("Không tìm thấy thông tin order");
            if (order.Status.Equals(InStoreOrderStatus.Paid)) throw new BadRequestException("Đơn hàng này đã thanh toán thành công rồi");

            var transaction = new Transaction
            {
                InStoreOrderId = inStoreOrderId,
                TotalAmount = model.Amount,
                Type = "Thanh toán hóa đơn tại cửa hàng Thanh Huy",
                PaymentMethod = "VNPay",
                Status = "Đang xử lý",
                Id = model.TxnRef
            };
            _transactionRepository.Add(transaction);
            return await _unitOfWork.SaveChanges() > 0;
        }

        public async Task<bool> ConfirmOrderPayment(VnPayResponseModel model)
        {
            var transaction = await _transactionRepository.GetMany(transaction => transaction.Id.Equals(model.TxnRef)).FirstOrDefaultAsync();
            if (transaction == null) throw new NotFoundException("Không tìm thấy thông tin của transaction");

            if (transaction.OnlineOrderId.HasValue)
            {
                var onlineOrder = await _onlineOrderRepository.GetMany(order => order.Id.Equals(transaction.OnlineOrderId)).FirstOrDefaultAsync();
                if (onlineOrder == null) return false;

                if(model.ResponseCode == "00")
                {
                    onlineOrder.Status = OnlineOrderStatus.Paid;
                    transaction.Status = "Thành công";
                }
                else
                {
                    transaction.Status = "Thất bại";
                }
                _onlineOrderRepository.Update(onlineOrder);
            }
            else
            {
                var inStoreOrder = await _inStoreOrderRepository.GetMany(order => order.Id.Equals(transaction.InStoreOrderId)).FirstOrDefaultAsync();
                if(inStoreOrder == null) return false;

                if(model.ResponseCode == "00")
                {
                    inStoreOrder.Status = InStoreOrderStatus.Paid;
                    transaction.Status = "Thành công";
                }
                else
                {
                    transaction.Status = "Thất bại";
                }
                _inStoreOrderRepository.Update(inStoreOrder);
            }
            transaction.UpdateAt = DateTime.UtcNow;
            _transactionRepository.Update(transaction);

            return await _unitOfWork.SaveChanges() > 0;
        }

        
    }
}
