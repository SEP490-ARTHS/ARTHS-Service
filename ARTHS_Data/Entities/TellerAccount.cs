using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class TellerAccount
    {
        public TellerAccount()
        {
            InStoreOrders = new HashSet<InStoreOrder>();
        }

        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string? Avatar { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<InStoreOrder> InStoreOrders { get; set; }
    }
}
