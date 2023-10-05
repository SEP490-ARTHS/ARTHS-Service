namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateInStoreOrderModel
    {
        public Guid StaffId { get; set; }
        public string? CustomerName { get; set; }
        public string CustomerPhone { get; set; } = null!;
        public string? LicensePlate { get; set; }
        public string OrderType { get; set; } = null!;

        public List<CreateInStoreOrderDetailModel> OrderDetailModel { get; set; } = new List<CreateInStoreOrderDetailModel>();
    }
}