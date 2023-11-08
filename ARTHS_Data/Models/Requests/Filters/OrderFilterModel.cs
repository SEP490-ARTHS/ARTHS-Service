using ARTHS_Utility.Enums;

namespace ARTHS_Data.Models.Requests.Filters
{
    public class OrderFilterModel
    {
        public Guid? StaffId { get; set; }
        public Guid? CustomerId { get; set; }

        public string? OrderId { get; set; }
        public string? CustomerName { get; set; }
        public string? OrderStatus { get; set; }
        public string? ExcludeOrderStatus { get; set; }
        public string? CustomerPhone { get; set; }
        public OrderType? OrderType { get; set; }
    }
}
