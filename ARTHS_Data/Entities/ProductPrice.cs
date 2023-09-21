using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class ProductPrice
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public DateTime DateApply { get; set; }
        public int PriceCurrent { get; set; }

        public virtual Product Product { get; set; } = null!;
    }
}
