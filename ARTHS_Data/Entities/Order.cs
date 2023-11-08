using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            RevenueStores = new HashSet<RevenueStore>();
        }

        public string Id { get; set; } = null!;
        public Guid? CustomerId { get; set; }
        public Guid? TellerId { get; set; }
        public Guid? StaffId { get; set; }
        public string? ShippingCode { get; set; }
        public int? ShippingMoney { get; set; }
        public string CustomerPhoneNumber { get; set; } = null!;
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        public string? PaymentMethod { get; set; }
        public string Status { get; set; } = null!;
        public int TotalAmount { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string? LicensePlate { get; set; }
        public string OrderType { get; set; } = null!;
        public DateTime OrderDate { get; set; }

        public virtual CustomerAccount? Customer { get; set; }
        public virtual StaffAccount? Staff { get; set; }
        public virtual TellerAccount? Teller { get; set; }
        public virtual RepairBooking? RepairBooking { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<RevenueStore> RevenueStores { get; set; }
    }
}
