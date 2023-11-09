using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Discount
    {
        public Discount()
        {
            MotobikeProducts = new HashSet<MotobikeProduct>();
            RepairServices = new HashSet<RepairService>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public int DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;

        public virtual ICollection<MotobikeProduct> MotobikeProducts { get; set; }
        public virtual ICollection<RepairService> RepairServices { get; set; }
    }
}
