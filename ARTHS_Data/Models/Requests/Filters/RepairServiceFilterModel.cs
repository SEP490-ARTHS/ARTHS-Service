using System.ComponentModel;

namespace ARTHS_Data.Models.Requests.Filters
{
    public class RepairServiceFilterModel
    {
        public string? Name { get; set; }

        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;  //Trang mặc định 1

        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;  //Mỗi trang có 10

    }
}
