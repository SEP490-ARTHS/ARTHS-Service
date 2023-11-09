namespace ARTHS_Data.Models.Views
{
    public class RepairBookingViewModel
    {
        public Guid Id { get; set; }
        public DateTime DateBook { get; set; }
        public string Description { get; set; } = null!;
        public string? CancellationReason { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual StaffViewModel? Staff { get; set; }
        public virtual BasicCustomerViewModel Customer { get; set; } = null!;
    }
}
