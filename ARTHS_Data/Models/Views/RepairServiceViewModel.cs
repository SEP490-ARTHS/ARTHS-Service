namespace ARTHS_Data.Models.Views
{
    public class RepairServiceViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public int Duration { get; set; }
        public int? ReminderInterval { get; set; }
        public int WarrantyDuration { get; set; }
        public int DiscountAmount { get; set; }
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual ICollection<ImageViewModel> Images { get; set; } = new List<ImageViewModel>();
    }
}
