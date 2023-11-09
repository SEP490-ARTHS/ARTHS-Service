using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class RepairService
    {
        public RepairService()
        {
            Images = new HashSet<Image>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int WarrantyDuration { get; set; }
        public Guid? DiscountId { get; set; }
        public int Duration { get; set; }
        public int? ReminderInterval { get; set; }
        public int Price { get; set; }
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual Discount? Discount { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
