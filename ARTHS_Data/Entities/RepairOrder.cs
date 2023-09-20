using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class RepairOrder
    {
        public RepairOrder()
        {
            RepairOrderDetails = new HashSet<RepairOrderDetail>();
        }

        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }

        public virtual StaffAccount Staff { get; set; } = null!;
        public virtual Bill? Bill { get; set; }
        public virtual ICollection<RepairOrderDetail> RepairOrderDetails { get; set; }
    }
}
