namespace ARTHS_Data.Models.Requests.Put
{
    public class UpdateRepairBookingModel
    {
        public string? TimeBook { get; set; }
        public string? DateBook { get; set; }
        public string? Description { get; set; }
        public string? CancellationReason { get; set; }
        public string? Status { get; set; }
        public Guid? StaffId { get; set; }

    }
}
