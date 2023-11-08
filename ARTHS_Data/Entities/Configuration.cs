using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Configuration
    {
        public int TotalStaff { get; set; }
        public int WorkHours { get; set; }
        public int ServiceTime { get; set; }
        public int NonBookingPercentage { get; set; }
        public int ShippingMoney { get; set; }
    }
}
