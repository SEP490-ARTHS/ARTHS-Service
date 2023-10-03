using ARTHS_Data.Entities;

namespace ARTHS_Data.Models.Views
{
    public class MotobikeProductDetailViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int PriceCurrent { get; set; }
        public int Quantity { get; set; }
        public int WarrantyDuration { get; set; }
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? UpdateAt { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual CategoryViewModel? Category { get; set; }
        public virtual DiscountViewModel? Discount { get; set; }
        public virtual RepairServiceViewModel? RepairService { get; set; }
        //public virtual ICollection<FeedbackProduct> FeedbackProducts { get; set; } = new List<FeedbackProduct>();
        public virtual ICollection<ImageViewModel> Images { get; set; } = new List<ImageViewModel>();
        public virtual ICollection<MotobikeProductPriceViewModel> MotobikeProductPrices { get; set; } = new List<MotobikeProductPriceViewModel>();

        public virtual ICollection<VehicleViewModel> Vehicles { get; set; } = new List<VehicleViewModel>();
    }
}
