﻿using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class InStoreOrder
    {
        public InStoreOrder()
        {
            InStoreOrderDetails = new HashSet<InStoreOrderDetail>();
            Transactions = new HashSet<Transaction>();
        }

        public string Id { get; set; } = null!;
        public Guid TellerId { get; set; }
        public Guid? StaffId { get; set; }
        public string? CustomerName { get; set; }
        public string CustomerPhone { get; set; } = null!;
        public string? LicensePlate { get; set; }
        public string Status { get; set; } = null!;
        public int TotalAmount { get; set; }
        public string OrderType { get; set; } = null!;
        public DateTime OrderDate { get; set; }

        public virtual StaffAccount? Staff { get; set; }
        public virtual TellerAccount Teller { get; set; } = null!;
        public virtual Bill? Bill { get; set; }
        public virtual ICollection<InStoreOrderDetail> InStoreOrderDetails { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
