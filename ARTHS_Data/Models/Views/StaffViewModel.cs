namespace ARTHS_Data.Models.Views
{
    public class StaffViewModel
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Avatar { get; set; }
    }
}
