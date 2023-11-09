namespace ARTHS_Data.Models.Views
{
    public class RevenueStoreViewModel
    {
        public string Id { get; set; } = null!;
        public string? OrderId { get; set; }
        public int TotalAmount { get; set; }
        public string Type { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? UpdateAt { get; set; }
        public DateTime TransactionDate { get; set; }

        //public virtual Order? Order { get; set; }
    }
}
