namespace ARTHS_Data.Models.Views
{
    public class MotobikeProductViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int PriceCurrent { get; set; }
        public int Quantity { get; set; }
        public int WarrantyDuration { get; set; }
        public string Status { get; set; } = null!;

        //public virtual CategoryViewModel? Category { get; set; }
        public int DiscountAmount { get; set; }
        //public virtual DiscountViewModel? Discount { get; set; }
        //public virtual BasicRepairServiceViewModel? RepairService { get; set; }

        public string ImageUrl { get; set; } = null!;
        //public virtual ICollection<ImageViewModel> Images { get; set; } = new List<ImageViewModel>();

        //public virtual Warranty? Warranty { get; set; }
    }
}
