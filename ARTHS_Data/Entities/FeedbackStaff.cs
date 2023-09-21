using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class FeedbackStaff
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid StaffId { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SendDate { get; set; }

        public virtual CustomerAccount? Customer { get; set; }
        public virtual StaffAccount Staff { get; set; } = null!;
    }
}
