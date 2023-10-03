using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class MotobikeProductPrice
    {
        public Guid Id { get; set; }
        public Guid MotobikeProductId { get; set; }
        public DateTime DateApply { get; set; }
        public int PriceCurrent { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual MotobikeProduct MotobikeProduct { get; set; } = null!;
    }
}
