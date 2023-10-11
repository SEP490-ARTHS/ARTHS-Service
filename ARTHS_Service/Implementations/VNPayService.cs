using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Helpers.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTHS_Service.Implementations
{
    public class VNPayService : BaseService, IVNPayService
    {
        private readonly IOnlineOrderRepository _onlineOrderRepository;
        private readonly ITransactionRepository _transactionRepository;

        public VNPayService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _onlineOrderRepository = unitOfWork.OnlineOrder;
            _transactionRepository = unitOfWork.Transactions;
        }

        public async Task<bool> ProcessOnlineOrderPayment(Guid onlineOrderId, VnPayRequestModel model)
        {
            var order = await _onlineOrderRepository.GetMany(order => order.Id.Equals(onlineOrderId)).FirstOrDefaultAsync();
            if (order == null) return false;

            var transaction = new Transaction
            {
                OnlineOrderId = onlineOrderId,
                TotalAmount = model.Amount,
                Type = "Thanh toán đơn hàng trực tuyến",
                PaymentMethod = "VNPay",
                Status = "Đang xử lý",
                Id = model.TxnRef
            };
            _transactionRepository.Add(transaction);
            return await _unitOfWork.SaveChanges() > 0;
        }

        public async Task<bool> ConfirmOnlineOrderPayment(VnPayResponseModel model)
        {
            var transaction = await _transactionRepository.GetMany(transaction => transaction.Id.Equals(model.TxnRef)).FirstOrDefaultAsync();
            if (transaction == null) return false;

            var order = await _onlineOrderRepository.GetMany(order => order.Id.Equals(transaction.OnlineOrderId)).FirstOrDefaultAsync();
            if (order == null) return false;

            order.Status = OnlineOrderStatus.Paid;
            transaction.Status = "Thành công";

            _onlineOrderRepository.Update(order);
            _transactionRepository.Update(transaction);

            return await _unitOfWork.SaveChanges() > 0;
        }
    }
}
