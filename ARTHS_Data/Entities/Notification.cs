using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Notification
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime SendDate { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
