using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class MaintenanceSchedule
    {
        public Guid Id { get; set; }
        public Guid OrderDetailId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime NextMaintenanceDate { get; set; }
        public DateTime ReminderDate { get; set; }
        public bool RemiderSend { get; set; }

        public virtual CustomerAccount Customer { get; set; } = null!;
        public virtual OrderDetail OrderDetail { get; set; } = null!;
    }
}
