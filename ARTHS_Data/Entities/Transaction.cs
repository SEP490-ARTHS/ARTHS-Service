﻿using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Transaction
    {
        public Guid Id { get; set; }
        public string? InStoreOrderId { get; set; }
        public Guid? OnlineOrderId { get; set; }
        public int TotalAmount { get; set; }
        public string Type { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? UpdateAt { get; set; }
        public DateTime TransactionDate { get; set; }

        public virtual InStoreOrder? InStoreOrder { get; set; }
        public virtual OnlineOrder? OnlineOrder { get; set; }
    }
}
