using ARTHS_Utility.Enums;

namespace ARTHS_Data.Models.Requests.Post
{
    public class PaymentModel
    {
        public Guid? OnlineOrderId { get; set; }
        public string? InStoreOrderId { get; set; }
        public int Amount { get; set; }
    }
}
