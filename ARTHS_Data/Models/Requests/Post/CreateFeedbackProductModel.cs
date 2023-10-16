namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateFeedbackProductModel
    {
        public Guid MotobikeProductId { get; set; }
        //public string? Title { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; } = null!;
    }
}
