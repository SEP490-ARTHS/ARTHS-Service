namespace ARTHS_Data.Models.Views
{
    public class CartItemViewModel
    {
        public Guid CartId { get; set; }
        public int Quantity { get; set; }

        public virtual BasicMotobikeProductViewModel MotobikeProduct { get; set; } = null!;
    }
}
