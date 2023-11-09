namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateWarrantyRequest
    {
        public int Duration { get; set; }
        public string Description { get; set; } = null!;
    }
}
