namespace ARTHS_Data.Models.Requests.Filters
{
    public class DiscountFilterModel
    {
        public string? Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? status { get; set; }
    }
}
