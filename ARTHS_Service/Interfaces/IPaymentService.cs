using ARTHS_Utility.Helpers.Models;
using ARTHS_Utility.Helpers.ZaloPay;

namespace ARTHS_Service.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessVnPayPayment(string inStoreOrderId, VnPayRequestModel model);
        Task<bool> ConfirmVnPayPayment(VnPayResponseModel model);

        Task<dynamic> ProcessZaloPayPayment(CreateZaloPayModel model);
        Task<dynamic> IsValidCallback(dynamic cbdata);
    }
}
