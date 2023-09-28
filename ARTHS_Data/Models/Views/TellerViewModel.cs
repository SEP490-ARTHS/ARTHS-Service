namespace ARTHS_Data.Models.Views
{
    public class TellerViewModel
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Avatar { get; set; }
    }
}
