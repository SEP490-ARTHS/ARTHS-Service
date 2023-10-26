namespace ARTHS_Data.Models.Views
{
    public class BasicCustomerViewModel
    {
        public Guid AccountId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Avatar { get; set; }
    }
}
