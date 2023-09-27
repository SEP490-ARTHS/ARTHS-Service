using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Vehicle
    {
        public Vehicle()
        {
            MotobikeProducts = new HashSet<MotobikeProduct>();
        }

        public Guid Id { get; set; }
        public string VehicleName { get; set; } = null!;

        public virtual ICollection<MotobikeProduct> MotobikeProducts { get; set; }
    }
}
