using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Image
    {
        public Guid Id { get; set; }
        public Guid? MotobikeProductId { get; set; }
        public Guid? RepairServiceId { get; set; }
        public bool Thumbnail { get; set; }
        public string ImageUrl { get; set; } = null!;

        public virtual MotobikeProduct? MotobikeProduct { get; set; }
        public virtual RepairService? RepairService { get; set; }
    }
}
