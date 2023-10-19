namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateRepairBookingModel
    {
        //public Guid CustomerId { get; set; }
        public string DateBook { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
