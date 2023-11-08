using ARTHS_Utility.Helpers.Models;

namespace ARTHS_Service.Interfaces
{
    public interface IVNPayService
    {
        Task<bool> ProcessInStoreOrderPayment(string inStoreOrderId, VnPayRequestModel model);
        Task<bool> ConfirmOrderPayment(VnPayResponseModel model);
    }
}
