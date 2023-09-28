using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Bill
    {
        public Guid Id { get; set; }
        public string InStoreOrderId { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public DateTime BillDate { get; set; }

        public virtual InStoreOrder InStoreOrder { get; set; } = null!;
    }
}
