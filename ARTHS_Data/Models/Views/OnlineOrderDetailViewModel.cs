namespace ARTHS_Data.Models.Views
{
    public class OnlineOrderDetailViewModel
    {
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int SubTotalAmount { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual BasicMotobikeProductViewModel MotobikeProduct { get; set; } = null!;
    }
}
