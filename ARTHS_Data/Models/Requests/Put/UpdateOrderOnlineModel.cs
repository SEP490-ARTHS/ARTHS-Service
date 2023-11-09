namespace ARTHS_Data.Models.Requests.Put
{
    public class UpdateOrderOnlineModel
    {
        public string Status { get; set; } = null!;
        public string? CancellationReason { get; set; }
    }
}
