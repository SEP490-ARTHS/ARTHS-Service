namespace ARTHS_Data.Models.Requests.Filters
{
    public class BookingFilterModel
    {
        public Guid? CustomerId { get; set; }
        public string? BookingDate { get; set; }
        public string? BookingStatus { get; set; }
    }
}
