using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class CustomerAccount
    {
        public CustomerAccount()
        {
            FeedbackProducts = new HashSet<FeedbackProduct>();
            FeedbackStaffs = new HashSet<FeedbackStaff>();
            MaintenanceSchedules = new HashSet<MaintenanceSchedule>();
            Orders = new HashSet<Order>();
            RepairBookings = new HashSet<RepairBooking>();
        }

        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string? Avatar { get; set; }
        public string Address { get; set; } = null!;

        public virtual Account Account { get; set; } = null!;
        public virtual Cart? Cart { get; set; }
        public virtual ICollection<FeedbackProduct> FeedbackProducts { get; set; }
        public virtual ICollection<FeedbackStaff> FeedbackStaffs { get; set; }
        public virtual ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RepairBooking> RepairBookings { get; set; }
    }
}
