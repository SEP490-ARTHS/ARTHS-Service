namespace ARTHS_Data.Models.Views
{
    public class BasicOrderViewModel
    {
        public string Id { get; set; } = null!;
        public Guid? CustomerId { get; set; }
        public Guid? TellerId { get; set; }
        public Guid? StaffId { get; set; }
        public string? ShippingCode { get; set; }
        public int? ShippingMoney { get; set; }
        public string CustomerPhoneNumber { get; set; } = null!;
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        public string? PaymentMethod { get; set; }
        public string Status { get; set; } = null!;
        public int TotalAmount { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string? LicensePlate { get; set; }
        public string OrderType { get; set; } = null!;
        public DateTime OrderDate { get; set; }
    }
}
