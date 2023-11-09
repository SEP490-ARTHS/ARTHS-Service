using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Helpers;
using ARTHS_Utility.Helpers.Models;
using ARTHS_Utility.Helpers.ZaloPay;
using ARTHS_Utility.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using System.Globalization;

namespace ARTHS_API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IRevenueStoreService _revenueStoreService;
        private readonly AppSetting _appSetting;

        public PaymentController(IPaymentService paymentService, IRevenueStoreService revenueStoreService, IOptions<AppSetting> appSettings)
        {
            _paymentService = paymentService;
            _revenueStoreService = revenueStoreService;
            _appSetting = appSettings.Value;
        }

        [HttpPost]
        [Route("vn-pay")]
        [Authorize(UserRole.Customer, UserRole.Teller)]
        [SwaggerOperation(Summary = "VnPay payment.")]
        public async Task<ActionResult<string>> CreateOnlineOrderPayment([FromBody] PaymentModel model)
        {
            bool result;
            var now = DateTime.UtcNow.AddHours(7);
            var clientIp = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "";
            var requestModel = new VnPayRequestModel
            {
                TxnRef = now.ToString("yyMMddHHmmssfff") + "_" + model.OrderId,
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
            result = await _paymentService.ProcessVnPayPayment(model.OrderId, requestModel);
            return result ? Ok(VnPayHelper.CreateRequestUrl(requestModel, _appSetting.VNPayUrl, _appSetting.MerchantPassword)) : BadRequest();
        }

        [HttpGet]
        [Route("ipn")]
        [SwaggerOperation(Summary = "VnPay auto callback.")]
        public async Task<IActionResult> VnPayIpnEntry([FromQuery] Dictionary<string, string> queryParams)
        {
            if (!VnPayHelper.ValidateSignature(_appSetting.MerchantPassword, queryParams))
            {
                return BadRequest("Invalid Signature.");
            }

            var model = VnPayHelper.ParseToResponseModel(queryParams);
            var result = await _paymentService.ConfirmVnPayPayment(model);
            return result ? Ok() : BadRequest();
        }

        [HttpGet]
        [Route("result")]
        [SwaggerOperation(Summary = "VnPay payment result.")]
        public async Task<ActionResult<PaymentViewModel>> PaymentResult([FromQuery] Dictionary<string, string> queryParams)
        {
            if (!VnPayHelper.ValidateSignature(_appSetting.MerchantPassword, queryParams))
            {
                return BadRequest("Invalid Signature.");
            }
            var model = VnPayHelper.ParseToResponseModel(queryParams);
            var transaction = await _revenueStoreService.GetRevenue(model.TxnRef);
            
            DateTime? payDate = model.PayDate is null ? null : DateTime.ParseExact(model.PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            return Ok(new PaymentViewModel
            {
                TransactionStatus = model.TransactionStatus,
                OrderId = transaction.OrderId!,
                Response = model.ResponseCode,
                OrderInfo = model.OrderInfo,
                BankCode = model.BankCode,
                Amount = model.Amount,
                CardType = model.CardType,
                PayDate = payDate,
                TransactionNo = model.TransactionNo,
            });
        }

        [HttpPost]
        [Route("zalo-pay")]
        [SwaggerOperation(Summary = "ZaloPay payment.")]
        public async Task<IActionResult> CreateZaloPay([FromBody] CreateZaloPayModel model)
        {
            return Ok(await _paymentService.ProcessZaloPayPayment(model));
        }

        [HttpPost]
        [Route("zalopay-callback")]
        [SwaggerOperation(Summary = "ZaloPay callback.")]
        public async Task<IActionResult> Post([FromBody] dynamic cbdata)
        {
            return Ok(await _paymentService.IsValidCallback(cbdata));
        }
    }
}
