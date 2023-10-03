namespace ARTHS_Data.Models.Views
{
    public class ImageViewModel
    {
        public Guid Id { get; set; }
        public bool Thumbnail { get; set; }
        public string ImageUrl { get; set; } = null!;
    }
}
