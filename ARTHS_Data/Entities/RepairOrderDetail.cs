using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class RepairOrderDetail
    {
        public Guid Id { get; set; }
        public Guid RepairOrderId { get; set; }
        public Guid? RepairServiceId { get; set; }
        public Guid? ProductId { get; set; }
        public int? ProductQuantity { get; set; }
        public int? ProductPrice { get; set; }
        public int? ServicePrice { get; set; }
        public DateTime WarrantyPeriod { get; set; }
        public int RepairCount { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual Product? Product { get; set; }
        public virtual RepairOrder RepairOrder { get; set; } = null!;
        public virtual RepairService? RepairService { get; set; }
    }
}
