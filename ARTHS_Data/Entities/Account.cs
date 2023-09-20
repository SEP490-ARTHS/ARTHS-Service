using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Account
    {
        public Account()
        {
            FeedbackProducts = new HashSet<FeedbackProduct>();
            FeedbackStaffCustomers = new HashSet<FeedbackStaff>();
            FeedbackStaffStaffs = new HashSet<FeedbackStaff>();
            Notifications = new HashSet<Notification>();
        }

        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual AccountRole Role { get; set; } = null!;
        public virtual Cart? Cart { get; set; }
        public virtual CustomerAccount? CustomerAccount { get; set; }
        public virtual OwnerAccount? OwnerAccount { get; set; }
        public virtual StaffAccount? StaffAccount { get; set; }
        public virtual ICollection<FeedbackProduct> FeedbackProducts { get; set; }
        public virtual ICollection<FeedbackStaff> FeedbackStaffCustomers { get; set; }
        public virtual ICollection<FeedbackStaff> FeedbackStaffStaffs { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
