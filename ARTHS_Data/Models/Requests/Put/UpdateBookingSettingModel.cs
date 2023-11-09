namespace ARTHS_Data.Models.Requests.Put
{
    public class UpdateBookingSettingModel
    {
        public int? TotalStaff { get; set; }
        public int? WorkHours { get; set; }
        public int? ServiceTime { get; set; }
        public int? NonBookingPercentage { get; set; }
    }
}
