using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Cart
    {
        public Cart()
        {
            CartItems = new HashSet<CartItem>();
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        public virtual CustomerAccount Customer { get; set; } = null!;
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
