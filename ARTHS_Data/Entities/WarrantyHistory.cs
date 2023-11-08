using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class WarrantyHistory
    {
        public Guid Id { get; set; }
        public Guid OrderDetailId { get; set; }
        public DateTime RepairDate { get; set; }
        public int ProductQuantity { get; set; }
        public string? RepairDetails { get; set; }
        public Guid? HandledBy { get; set; }
        public int? TotalAmount { get; set; }
        public string Status { get; set; } = null!;

        public virtual StaffAccount? HandledByNavigation { get; set; }
        public virtual OrderDetail OrderDetail { get; set; } = null!;
    }
}
