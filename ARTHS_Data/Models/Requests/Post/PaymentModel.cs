namespace ARTHS_Data.Models.Requests.Post
{
    public class PaymentModel
    {
        public string OrderId { get; set; } = null!;
        public int Amount { get; set; }
    }
}
