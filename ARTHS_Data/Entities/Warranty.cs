using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Warranty
    {
        public Warranty()
        {
            Products = new HashSet<Product>();
        }

        public Guid Id { get; set; }
        public int Duration { get; set; }
        public string Term { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
