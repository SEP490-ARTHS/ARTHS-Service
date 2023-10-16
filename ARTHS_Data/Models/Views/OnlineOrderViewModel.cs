using ARTHS_Data.Entities;

namespace ARTHS_Data.Models.Views
{
    public class OnlineOrderViewModel
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int TotalAmount { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancellationDate { get; set; }
        public DateTime OrderDate { get; set; }

        //public virtual CustomerAccount Customer { get; set; } = null!;
        public virtual ICollection<OnlineOrderDetailViewModel> OnlineOrderDetails { get; set; } = new List<OnlineOrderDetailViewModel>();
    }
}
