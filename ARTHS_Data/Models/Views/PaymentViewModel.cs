namespace ARTHS_Data.Models.Views
{
    public class PaymentViewModel
    {
        public string OrderInfo { get; set; } = "";
        public string OrderId { get; set; } = "";
        public int Amount { get; set; }
        public string BankCode { get; set; } = "";
        public string? CardType { get; set; } = "";
        public string Response { get; set; } = "";
        public string TransactionStatus { get; set; } = "";
        public string TransactionNo { get; set; } = "";
        public DateTime? PayDate { get; set; }
    }
}
