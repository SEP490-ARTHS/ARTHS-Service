using System.ComponentModel;

namespace ARTHS_Data.Models.Requests.Filters
{
    public class DiscountFilterModel
    {
        public string? Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;  //Trang mặc định 1

        [DefaultValue(5)]
        public int PageSize { get; set; } = 5;  //Mỗi trang có 10
    }
}
