namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateOrderOfflineModel
    {
        public Guid? StaffId { get; set; }
        public Guid? BookingId { get; set; }

        public string CustomerPhoneNumber { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string? LicensePlate { get; set; }


        public List<CreateOrderOfflineDetailModel> OrderDetailModel { get; set; } = new List<CreateOrderOfflineDetailModel>();
    }
}
