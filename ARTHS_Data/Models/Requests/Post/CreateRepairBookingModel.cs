namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateRepairBookingModel
    {
        public string DateBook { get; set; } = null!;
        public string Description { get; set; } = null!;

        public Guid? StaffId { get; set; }

    }
}
