using ARTHS_Data.Entities;

namespace ARTHS_Data.Models.Views
{
    public class OrderDetailViewModel
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public bool InstUsed { get; set; }
        public int SubTotalAmount { get; set; }
        public DateTime? WarrantyStartDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual BasicMotobikeProductViewModel? MotobikeProduct { get; set; }
        public virtual BasicRepairServiceViewModel? RepairService { get; set; }
        //public virtual MaintenanceSchedule? MaintenanceSchedule { get; set; }
        public virtual ICollection<WarrantyHistoryViewModel> WarrantyHistories { get; set; } = new List<WarrantyHistoryViewModel>();
    }
}
