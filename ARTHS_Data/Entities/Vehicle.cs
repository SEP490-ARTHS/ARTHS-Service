using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Vehicle
    {
        public Vehicle()
        {
            Products = new HashSet<Product>();
        }

        public Guid Id { get; set; }
        public string VehicleName { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
