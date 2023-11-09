namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateOrderOfflineDetailModel
    {
        public Guid? RepairServiceId { get; set; }
        public Guid? MotobikeProductId { get; set; }
        public int? ProductQuantity { get; set; }
        public bool? InstUsed { get; set; }
    }
}
