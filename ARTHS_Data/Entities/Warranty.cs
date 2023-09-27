using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Warranty
    {
        public Warranty()
        {
            MotobikeProducts = new HashSet<MotobikeProduct>();
        }

        public Guid Id { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; } = null!;

        public virtual ICollection<MotobikeProduct> MotobikeProducts { get; set; }
    }
}
