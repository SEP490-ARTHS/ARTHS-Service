using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            WarrantyHistories = new HashSet<WarrantyHistory>();
        }

        public Guid Id { get; set; }
        public string OrderId { get; set; } = null!;
        public Guid? MotobikeProductId { get; set; }
        public Guid? RepairServiceId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public bool InstUsed { get; set; }
        public int SubTotalAmount { get; set; }
        public DateTime? WarrantyStartDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual MotobikeProduct? MotobikeProduct { get; set; }
        public virtual Order Order { get; set; } = null!;
        public virtual RepairService? RepairService { get; set; }
        public virtual MaintenanceSchedule? MaintenanceSchedule { get; set; }
        public virtual ICollection<WarrantyHistory> WarrantyHistories { get; set; }
    }
}
