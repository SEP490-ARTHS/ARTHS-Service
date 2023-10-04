using ARTHS_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTHS_Data.Models.Views
{
    public class InStoreOrderDetailViewModel
    {
        public Guid Id { get; set; }
        //public string InStoreOrderId { get; set; } = null!;
        //public Guid? RepairServiceId { get; set; }
        //public Guid? MotobikeProductId { get; set; }
        public int? ProductQuantity { get; set; }
        public int? ProductPrice { get; set; }
        public int? ServicePrice { get; set; }
        public DateTime WarrantyPeriod { get; set; }
        public int RepairCount { get; set; }
        public DateTime CreateAt { get; set; }

        //public virtual InStoreOrder InStoreOrder { get; set; } = null!;
        public virtual BasicMotobikeProductViewModel? MotobikeProduct { get; set; }
        public virtual BasicRepairServiceViewModel? RepairService { get; set; }
    }
}
