namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateOnlineOrderModel
    {
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
    }
}
