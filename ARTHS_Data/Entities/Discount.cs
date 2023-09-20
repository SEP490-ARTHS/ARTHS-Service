using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Discount
    {
        public Discount()
        {
            Products = new HashSet<Product>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public int DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
