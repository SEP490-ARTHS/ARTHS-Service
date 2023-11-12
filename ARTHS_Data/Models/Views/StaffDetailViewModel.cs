namespace ARTHS_Data.Models.Views
{
    public class StaffDetailViewModel
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Avatar { get; set; }
        public virtual ICollection<FeedbackStaffViewModel> FeedbackStaffs { get; set; } = new List<FeedbackStaffViewModel>();

    }
}
