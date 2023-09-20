using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Bill
    {
        public Guid Id { get; set; }
        public Guid RepairOrderId { get; set; }
        public int TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public DateTime BillDate { get; set; }

        public virtual RepairOrder RepairOrder { get; set; } = null!;
    }
}
