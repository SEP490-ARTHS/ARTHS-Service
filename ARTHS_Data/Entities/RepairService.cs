using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class RepairService
    {
        public RepairService()
        {
            Products = new HashSet<Product>();
            RepairOrderDetails = new HashSet<RepairOrderDetail>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? UpdateAt { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<RepairOrderDetail> RepairOrderDetails { get; set; }
    }
}
