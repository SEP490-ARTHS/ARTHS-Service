using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class MotobikeProductImage
    {
        public Guid Id { get; set; }
        public Guid MotobikeProductId { get; set; }
        public bool Thumbnail { get; set; }
        public string ImageUrl { get; set; } = null!;

        public virtual MotobikeProduct MotobikeProduct { get; set; } = null!;
    }
}
