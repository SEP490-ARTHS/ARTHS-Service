using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTHS_Data.Models.Views
{
    public class BookingSettingViewModel
    {
        public int TotalStaff { get; set; }
        public int WorkHours { get; set; }
        public int ServiceTime { get; set; }
        public int NonBookingPercentage { get; set; }
        public int DailyOnlineBookings { get; set; }
        
    }
}
