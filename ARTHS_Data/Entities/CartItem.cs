﻿using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class CartItem
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual Cart Cart { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}