
namespace ARTHS_Data.Models.Requests.Filters
{
    public class RepairServiceFilterModel
    {
        public string? Name { get; set; }
        public string? Status { get; set; }
        public bool? SortByNameAsc { get; set; }
        public bool? SortByPriceAsc { get; set; }
    }
}
