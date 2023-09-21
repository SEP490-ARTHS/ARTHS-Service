using ARTHS_Data.Entities;

namespace ARTHS_Data.Models.Views
{
    public class AccountViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual RoleViewModel Role { get; set; } = null!;
        public virtual Cart? Cart { get; set; }
        public virtual CustomerViewModel? CustomerAccount { get; set; }
        public virtual OwnerAccount? OwnerAccount { get; set; }
        public virtual StaffAccount? StaffAccount { get; set; }
    }
}
