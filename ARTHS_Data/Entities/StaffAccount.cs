using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class StaffAccount
    {
        public StaffAccount()
        {
            FeedbackStaffs = new HashSet<FeedbackStaff>();
            InStoreOrders = new HashSet<InStoreOrder>();
            OnlineOrders = new HashSet<OnlineOrder>();
        }

        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string? Avatar { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<FeedbackStaff> FeedbackStaffs { get; set; }
        public virtual ICollection<InStoreOrder> InStoreOrders { get; set; }
        public virtual ICollection<OnlineOrder> OnlineOrders { get; set; }
    }
}
