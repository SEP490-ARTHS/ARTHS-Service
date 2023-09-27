using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class OnlineOrderDetail
    {
        public Guid OnlineOrderId { get; set; }
        public Guid MotobikeProductId { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int SubTotalAmount { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual MotobikeProduct MotobikeProduct { get; set; } = null!;
        public virtual OnlineOrder OnlineOrder { get; set; } = null!;
    }
}
