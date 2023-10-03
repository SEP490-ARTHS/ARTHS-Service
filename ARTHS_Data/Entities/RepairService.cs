using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class RepairService
    {
        public RepairService()
        {
            Images = new HashSet<Image>();
            InStoreOrderDetails = new HashSet<InStoreOrderDetail>();
            MotobikeProducts = new HashSet<MotobikeProduct>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<InStoreOrderDetail> InStoreOrderDetails { get; set; }
        public virtual ICollection<MotobikeProduct> MotobikeProducts { get; set; }
    }
}
