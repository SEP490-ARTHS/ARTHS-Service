using System.ComponentModel;

namespace ARTHS_Data.Models.Requests.Filters
{
    public class InStoreOrderFilterModel
    {
        public Guid? StaffId { get; set; }
        public string? CustomerName { get; set; }
        public string? OrderStatus { get; set; }
        public string? ExcludeOrderStatus { get; set; }
        public string? CustomerPhone { get; set; }
    }
}
