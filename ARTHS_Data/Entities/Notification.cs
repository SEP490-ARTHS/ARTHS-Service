using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Notification
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string? Type { get; set; }
        public string? Link { get; set; }
        public bool IsRead { get; set; }
        public DateTime SendDate { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
