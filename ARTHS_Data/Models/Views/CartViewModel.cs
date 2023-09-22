namespace ARTHS_Data.Models.Views
{
    public class CartViewModel
    {
        public Guid Id { get; set; }
        public virtual ICollection<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
    }
}
