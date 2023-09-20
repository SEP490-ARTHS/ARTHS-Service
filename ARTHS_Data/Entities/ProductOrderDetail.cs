using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class ProductOrderDetail
    {
        public Guid Id { get; set; }
        public Guid ProductOrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual ProductOrder ProductOrder { get; set; } = null!;
    }
}
