namespace ARTHS_Data.Models.Views
{
    public class BasicInStoreOrderViewModel
    {
        public string Id { get; set; } = null!;
        public string TellerName { get; set; } = null!;
        public string StaffName { get; set; } = null!;
        public string? CustomerName { get; set; }
        public string CustomerPhone { get; set; } = null!;
        public string? LicensePlate { get; set; }
        public string Status { get; set; } = null!;
        public int TotalAmount { get; set; }
        public string OrderType { get; set; } = null!;
        public DateTime OrderDate { get; set; }
    }
}
