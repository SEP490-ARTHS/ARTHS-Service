using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Exceptions;
using ARTHS_Utility.Helpers.Models;
using ARTHS_Utility.Helpers.ZaloPay;
using ARTHS_Utility.Settings;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ARTHS_Service.Implementations
{
    public class PaymentService : BaseService, IPaymentService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRevenueStoreRepository _revenueStoreRepository;
        private readonly AppSetting _appSettings;

        private const string CREATE_ORDER_URL = "https://sb-openapi.zalopay.vn/v2/create";
        private const string QUERY_ORDER_URL = "https://sb-openapi.zalopay.vn/v2/query";
        private const string CallBack_URL = "https://thanh-huy.azurewebsites.net/api/payments/zalopay-callback";

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<AppSetting> appSettings) : base(unitOfWork, mapper)
        {
            _orderRepository = unitOfWork.Order;
            _revenueStoreRepository = unitOfWork.RevenueStore;

            _appSettings = appSettings.Value;
        }


        public async Task<bool> ProcessVnPayPayment(string orderId, VnPayRequestModel model)
        {
            var order = await _orderRepository.GetMany(order => order.Id.Equals(orderId)).FirstOrDefaultAsync();
            if (order == null) throw new NotFoundException("Không tìm thấy thông tin order");
            if (order.Status.Equals(OrderStatus.Paid)) throw new BadRequestException("Đơn hàng này đã thanh toán thành công rồi");
            var existRevenues = await _revenueStoreRepository.GetMany(revenue => revenue.OrderId!.Equals(orderId)).ToListAsync();
            if(existRevenues != null)
            {
                _revenueStoreRepository.RemoveRange(existRevenues);
            }
            var revenue = new RevenueStore
            {
                OrderId = orderId,
                TotalAmount = model.Amount,
                Type = "Thanh toán hóa đơn cửa hàng Thanh Huy",
                PaymentMethod = PaymentMethods.VNPay,
                Status = "Đang xử lý",
                Id = model.TxnRef
            };
            _revenueStoreRepository.Add(revenue);
            return await _unitOfWork.SaveChanges() > 0;
        }

        public async Task<bool> ConfirmVnPayPayment(VnPayResponseModel model)
        {
            var revenue = await _revenueStoreRepository.GetMany(transaction => transaction.Id.Equals(model.TxnRef)).FirstOrDefaultAsync();
            if (revenue == null) throw new NotFoundException("Không tìm thấy thông tin của revenue");
            var order = await _orderRepository.GetMany(order => order.Id.Equals(revenue.OrderId)).FirstOrDefaultAsync();
            if (order == null) return false;
            if (model.ResponseCode == "00")
            {
                order.Status = OrderStatus.Paid;
                order.PaymentMethod = PaymentMethods.VNPay;
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


        public async Task<dynamic> ProcessZaloPayPayment(CreateZaloPayModel model)
        {
            var order = await _orderRepository.GetMany(order => order.Id.Equals(model.OrderId)).Include(order => order.OrderDetails).FirstOrDefaultAsync();
            if (order == null) throw new NotFoundException("Không tìm thấy thông tin order.");
            
            var items = order.OrderDetails.Select(detail => new
            {
                itemname = detail.MotobikeProduct?.Name ?? detail.RepairService?.Name,
                itemprice = detail.Price,
                itemquantity = detail.Quantity
            }).ToArray();

            var embed_data = new {
                merchantinfo = "Thanh Huy Motorbike"
            };
            DateTime now = DateTime.UtcNow.AddHours(7);
            string AppTransId = now.ToString("yyMMddHHmmssfff") + "_" + model.OrderId;

            var existRevenues = await _revenueStoreRepository.GetMany(revenue => revenue.OrderId!.Equals(order.Id)).ToListAsync();
            if (existRevenues != null)
            {
                _revenueStoreRepository.RemoveRange(existRevenues);
            }
            var revenue = new RevenueStore
            {
                Id = AppTransId,
                OrderId = model.OrderId,
                TotalAmount = order.TotalAmount,
                Type = "Thanh toán hóa đơn cửa hàng Thanh Huy",
                PaymentMethod = PaymentMethods.ZaloPay,
                Status = "Đang xử lý"
            };
            _revenueStoreRepository.Add(revenue);
            await _unitOfWork.SaveChanges();

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long unixTime = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;


            var param = new Dictionary<string, string>
                {
                { "app_id", _appSettings.AppId.ToString() },
                { "app_user", order.CustomerPhoneNumber },
                { "app_time", unixTime.ToString() },
                { "amount", order.TotalAmount.ToString() },
                { "app_trans_id", AppTransId },
                { "embed_data", JsonConvert.SerializeObject(embed_data) },
                { "item", JsonConvert.SerializeObject(items) },
                { "description", $"ThanhHuy - Thanh toán đơn hàng #{model.OrderId}" },
                { "bank_code", "zalopayapp" },
                {"callback_url", CallBack_URL }
                };

            //https://localhost:7235/api/payments/zalopay-callback
            var data = string.Join("|", _appSettings.AppId, param["app_trans_id"], param["app_user"], param["amount"],
                                    param["app_time"], param["embed_data"], param["item"]);

            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, _appSettings.Key1, data));
            //Console.WriteLine(JsonConvert.SerializeObject(param, Formatting.Indented));
            var result = await HttpHelper.PostFormAsync(CREATE_ORDER_URL, param);
            return result;
        }

        public async Task<dynamic> IsValidCallback(dynamic cbdata)
        {
            string key2 = _appSettings.Key2;
            var result = new Dictionary<string, object>();

            try
            {
                Console.WriteLine("Da vao call back");

                var dataStr = Convert.ToString(cbdata["data"]);
                var reqMac = Convert.ToString(cbdata["mac"]);

                // Parse the nested JSON string
                var dataJson = JsonConvert.DeserializeObject<dynamic>(dataStr);
                var appTransId = Convert.ToString(dataJson["app_trans_id"]);

                var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key2, dataStr);

                Console.WriteLine("mac = {0}", mac);

                // kiểm tra callback hợp lệ (đến từ ZaloPay server)
                if (!reqMac.Equals(mac))
                {
                    Console.WriteLine("Lỗi");

                    // callback không hợp lệ
                    result["return_code"] = -1;
                    result["return_message"] = "mac not equal";
                }
                else
                {
                    string id = appTransId;
                    var revenue = await _revenueStoreRepository.GetMany(tran => tran.Id.Equals(id)).FirstOrDefaultAsync();
                    if (revenue == null) throw new NotFoundException("Không tìm thấy thông tin của revenue");

                    var onlineOrder = await _orderRepository.GetMany(order => order.Id.Equals(revenue.OrderId)).FirstOrDefaultAsync();
                    if (onlineOrder == null) return false;

                    onlineOrder.Status = OrderStatus.Paid;
                    onlineOrder.PaymentMethod = PaymentMethods.ZaloPay;
                    revenue.Status = "Thành công";
                    _orderRepository.Update(onlineOrder);

                    revenue.UpdateAt = DateTime.UtcNow;
                    _revenueStoreRepository.Update(revenue);
                    await _unitOfWork.SaveChanges();

                    // thanh toán thành công
                    // merchant cập nhật trạng thái cho đơn hàng
                    //var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
                    //Console.WriteLine("update order's status = success where app_trans_id = {0}", dataJson["app_trans_id"]);

                    result["return_code"] = 1;
                    result["return_message"] = "success";
                }
            }
            catch (Exception ex)
            {
                result["return_code"] = 0; // ZaloPay server sẽ callback lại (tối đa 3 lần)
                result["return_message"] = ex.Message;
            }
            return result;
        }

    }
}
