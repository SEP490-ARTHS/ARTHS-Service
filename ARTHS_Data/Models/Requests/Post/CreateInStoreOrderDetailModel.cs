namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateInStoreOrderDetailModel
    {
        public Guid? RepairServiceId { get; set; }
        public Guid? MotobikeProductId { get; set; }
        public int? ProductQuantity { get; set; }
    }
}
