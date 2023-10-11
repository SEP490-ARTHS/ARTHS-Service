using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
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
        private readonly AppSetting _appSetting;

        public PaymentController(IVNPayService vnPayService, IOptions<AppSetting> appSettings)
        {
            _vnPayService = vnPayService;
            _appSetting = appSettings.Value;
        }

        [HttpPost]
        [Route("online-orders")]
        [Authorize(UserRole.Customer, UserRole.Teller)]
        public async Task<ActionResult<string>> CreateOnlineOrderPayment(PaymentModel model)
        {
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
                OrderInfo = $"Thanh toan hoa don: {model.Amount} VND",
                IpAddress = clientIp,
                ReturnUrl = _appSetting.ReturnUrl,
                TmnCode = _appSetting.MerchantId
            };

            var result = await _vnPayService.ProcessOnlineOrderPayment(model.OnlineOrderId, requestModel);
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
            var result = await _vnPayService.ConfirmOnlineOrderPayment(model);
            return result ? Ok() : BadRequest();
        }

        [HttpGet]
        [Route("result")]
        public ActionResult<PaymentViewModel> PaymentResult([FromQuery] Dictionary<string, string> queryParams)
        {
            if(!VnPayHelper.ValidateSignature(_appSetting.MerchantPassword, queryParams))
            {
                return BadRequest("Invalid Signature.");
            }
            var model = VnPayHelper.ParseToResponseModel(queryParams);

            DateTime? payDate = model.PayDate is null ? null : DateTime.ParseExact(model.PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            return Ok(new PaymentViewModel
            {
                TransactionStatus = model.TransactionStatus,
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
