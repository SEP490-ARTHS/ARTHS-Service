using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class OwnerAccount
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
