using ARTHS_Utility.Helpers.Models;

namespace ARTHS_Service.Interfaces
{
    public interface IGhnService
    {
        Task<GhnCreateResponseModel> CreateShippingOrder(GhnCreateOrderModel model);
        Task GhnCallBack(GhnWebHookResponse model);
    }
}
