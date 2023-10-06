using ARTHS_Data.Entities;

namespace ARTHS_Data.Models.Views
{
    public class AccountViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string? Avatar { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        public string Role { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

    }
}
