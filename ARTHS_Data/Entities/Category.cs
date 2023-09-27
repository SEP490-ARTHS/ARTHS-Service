using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Category
    {
        public Category()
        {
            MotobikeProducts = new HashSet<MotobikeProduct>();
        }

        public Guid Id { get; set; }
        public string CategoryName { get; set; } = null!;

        public virtual ICollection<MotobikeProduct> MotobikeProducts { get; set; }
    }
}
