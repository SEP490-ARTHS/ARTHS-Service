using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class FeedbackProduct
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid MotobikeProductId { get; set; }
        public string? Title { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; } = null!;
        public DateTime? UpdateAt { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual CustomerAccount? Customer { get; set; }
        public virtual MotobikeProduct MotobikeProduct { get; set; } = null!;
    }
}
