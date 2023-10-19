using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Enums;
using ARTHS_Utility.Exceptions;
using ARTHS_Utility.Helpers;
using ARTHS_Utility.Helpers.Models;
using ARTHS_Utility.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace ARTHS_API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVNPayService _vnPayService;
        private readonly ITransactionService _transactionService;
        private readonly AppSetting _appSetting;

        public PaymentController(IVNPayService vnPayService, ITransactionService transactionService ,IOptions<AppSetting> appSettings)
        {
            _vnPayService = vnPayService;
            _transactionService = transactionService;
            _appSetting = appSettings.Value;
        }

        [HttpPost]
        [Route("vn-pay")]
        [Authorize(UserRole.Customer, UserRole.Teller)]
        public async Task<ActionResult<string>> CreateOnlineOrderPayment([FromBody]PaymentModel model)
        {
            if(model.InStoreOrderId == null && model.OnlineOrderId == null)
            {
                throw new BadRequestException("Vui lòng nhập order id.");
            }
            if(model.InStoreOrderId != null && model.OnlineOrderId != null)
            {
                throw new BadRequestException("Vui lòng chỉ nhập 1 order id.");
            }


            var now = DateTime.UtcNow.AddHours(7);
            var clientIp = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "";
            var requestModel = new VnPayRequestModel
            {
                TxnRef = Guid.NewGuid(),
                Command = VnPayConstant.Command,
                Locale = VnPayConstant.Locale,
                Version = VnPayConstant.Version,
                CurrencyCode = VnPayConstant.CurrencyCode,
                Amount = model.Amount,
                CreateDate = now,
                ExpireDate = now.AddMinutes(15),
                OrderInfo = $"Thanh toán hóa đơn cửa hàng Thanh Huy. Tổng tiền: {model.Amount} VNĐ",
                IpAddress = clientIp,
                ReturnUrl = _appSetting.ReturnUrl,
                TmnCode = _appSetting.MerchantId,
                OrderType = "Other"
            };

            bool result;
            if (model.OnlineOrderId != null)
            {
                result = await _vnPayService.ProcessOnlineOrderPayment((Guid)model.OnlineOrderId, requestModel);
            }
            else
            {
                result = await _vnPayService.ProcessInStoreOrderPayment(model.InStoreOrderId!, requestModel);
            }

            return result ? Ok(VnPayHelper.CreateRequestUrl(requestModel, _appSetting.VNPayUrl, _appSetting.MerchantPassword)) : BadRequest();
        }

        [HttpGet]
        [Route("ipn")]
        public async Task<IActionResult> VnPayIpnEntry([FromQuery] Dictionary<string, string> queryParams)
        {
            if (!VnPayHelper.ValidateSignature(_appSetting.MerchantPassword, queryParams))
            {
                return BadRequest("Invalid Signature.");
            }

            var model = VnPayHelper.ParseToResponseModel(queryParams);
            var result = await _vnPayService.ConfirmOrderPayment(model);
            return result ? Ok() : BadRequest();
        }

        [HttpGet]
        [Route("result")]
        public async Task<ActionResult<PaymentViewModel>> PaymentResult([FromQuery] Dictionary<string, string> queryParams)
        {
            if(!VnPayHelper.ValidateSignature(_appSetting.MerchantPassword, queryParams))
            {
                return BadRequest("Invalid Signature.");
            }
            var model = VnPayHelper.ParseToResponseModel(queryParams);
            var transaction = await _transactionService.GetTransaction(model.TxnRef);
            var orderId = transaction.InStoreOrderId ?? transaction.OnlineOrderId.ToString();
            if(transaction.InStoreOrderId != null)
            {
                orderId = transaction.InStoreOrderId;
            }
            else
            {
                orderId = transaction.OnlineOrderId.ToString();
            }
            DateTime? payDate = model.PayDate is null ? null : DateTime.ParseExact(model.PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            return Ok(new PaymentViewModel
            {
                TransactionStatus = model.TransactionStatus,
                OrderId = orderId!,
                Response = model.ResponseCode,
                OrderInfo = model.OrderInfo,
                BankCode = model.BankCode,
                Amount = model.Amount,
                CardType = model.CardType,
                PayDate = payDate,
                TransactionNo = model.TransactionNo,
            });
        }
    }
}
