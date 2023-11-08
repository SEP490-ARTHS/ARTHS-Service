using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class RepairBooking
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? StaffId { get; set; }
        public string? OrderId { get; set; }
        public DateTime DateBook { get; set; }
        public string Description { get; set; } = null!;
        public string? CancellationReason { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual CustomerAccount Customer { get; set; } = null!;
        public virtual Order? Order { get; set; }
        public virtual StaffAccount? Staff { get; set; }
    }
}
