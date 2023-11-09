using ARTHS_Data.Models.Requests.Post;

namespace ARTHS_Data.Models.Requests.Put
{
    public class UpdateInStoreOrderModel
    {
        public Guid? StaffId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? LicensePlate { get; set; }
        public string? Status { get; set; }


        public List<CreateOrderOfflineDetailModel>? OrderDetailModel { get; set; } = new List<CreateOrderOfflineDetailModel>();
    }
}
