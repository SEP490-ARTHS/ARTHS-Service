using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Account
    {
        public Account()
        {
            Notifications = new HashSet<Notification>();
        }

        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Avatar { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual AccountRole Role { get; set; } = null!;
        public virtual CustomerAccount? CustomerAccount { get; set; }
        public virtual OwnerAccount? OwnerAccount { get; set; }
        public virtual StaffAccount? StaffAccount { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
