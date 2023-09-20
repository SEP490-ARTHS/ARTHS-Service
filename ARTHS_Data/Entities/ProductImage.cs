using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class ProductImage
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public bool Thumbnail { get; set; }
        public string ImageUrl { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}
