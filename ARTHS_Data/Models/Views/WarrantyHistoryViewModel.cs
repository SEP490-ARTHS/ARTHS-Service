namespace ARTHS_Data.Models.Views
{
    public class WarrantyHistoryViewModel
    {
        public Guid Id { get; set; }
        public DateTime RepairDate { get; set; }
        public int ProductQuantity { get; set; }
        public string? RepairDetails { get; set; }
        //public Guid? HandledBy { get; set; }
        public int? TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public virtual StaffViewModel? HandledByNavigation { get; set; }
    }
}
