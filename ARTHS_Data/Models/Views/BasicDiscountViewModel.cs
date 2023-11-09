namespace ARTHS_Data.Models.Views
{
    public class BasicDiscountViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public int DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
