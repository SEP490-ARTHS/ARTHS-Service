namespace ARTHS_Data.Models.Views
{
    public class BasicOnlineOrderViewModel
    {
        public Guid Id { get; set; }
        public string? CustomerName { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int TotalAmount { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancellationDate { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
