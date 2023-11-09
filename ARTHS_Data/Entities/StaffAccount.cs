using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class StaffAccount
    {
        public StaffAccount()
        {
            FeedbackStaffs = new HashSet<FeedbackStaff>();
            Orders = new HashSet<Order>();
            RepairBookings = new HashSet<RepairBooking>();
            WarrantyHistories = new HashSet<WarrantyHistory>();
        }

        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string? Avatar { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<FeedbackStaff> FeedbackStaffs { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RepairBooking> RepairBookings { get; set; }
        public virtual ICollection<WarrantyHistory> WarrantyHistories { get; set; }
    }
}
