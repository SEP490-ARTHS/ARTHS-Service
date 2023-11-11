namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateFeedbackStaffModel
    {
        public Guid? CustomerId { get; set; }
        public Guid StaffId { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; } = null!;
    }
}
