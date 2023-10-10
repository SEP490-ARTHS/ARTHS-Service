namespace ARTHS_Data.Models.Requests.Put
{
    public class UpdateOnlineOrderModel
    {
        public string Status { get; set; } = null!;
        public string? CancellationReason { get; set; }
    }
}
