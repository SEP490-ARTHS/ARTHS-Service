namespace ARTHS_Data.Models.Views
{
    public class FeedbackStaffViewModel
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SendDate { get; set; }

        public virtual BasicCustomerViewModel? Customer { get; set; }
    }
}
