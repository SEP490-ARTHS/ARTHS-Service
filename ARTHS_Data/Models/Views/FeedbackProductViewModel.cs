namespace ARTHS_Data.Models.Views
{
    public class FeedbackProductViewModel
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; } = null!;
        public DateTime? UpdateAt { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual BasicCustomerViewModel? Customer { get; set; }
    }
}
