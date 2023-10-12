namespace ARTHS_Data.Models.Views
{
    public class TransactionViewModel
    {
        public Guid Id { get; set; }
        public int TotalAmount { get; set; }
        public string Type { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? UpdateAt { get; set; }
        public DateTime TransactionDate { get; set; }

        //public virtual BasicInStoreOrderViewModel? InStoreOrder { get; set; }
        //public virtual BasicOnlineOrderViewModel? OnlineOrder { get; set; }
    }
}
