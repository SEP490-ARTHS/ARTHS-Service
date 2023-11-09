namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateWarrantyModel
    {
        public Guid OrderDetailId { get; set; }
        public Guid HandledBy { get; set; }
        public int? ProductQuantity { get; set; }
        public string? RepairDetails { get; set; }
        public int TotalAmount { get; set; }
    }
}
