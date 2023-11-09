namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateOrderOnlineModel
    {
        public string CustomerPhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public List<CreateOrderOnlineDetailModel> OrderDetailModels { get; set; } = new List<CreateOrderOnlineDetailModel>();
    }
}
