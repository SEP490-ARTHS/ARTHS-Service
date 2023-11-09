namespace ARTHS_Data.Models.Requests.Put
{
    public class UpdateWarrantyModel
    {
        public int? ProductQuantity { get; set; }
        public string? RepairDetails { get; set; }
        public string? Status { get; set; }

        public int? TotalAmount { get; set; }
    }
}
