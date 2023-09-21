using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class ProductOrder
    {
        public ProductOrder()
        {
            ProductOrderDetails = new HashSet<ProductOrderDetail>();
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? StaffId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }

        public virtual CustomerAccount Customer { get; set; } = null!;
        public virtual StaffAccount? Staff { get; set; }
        public virtual ICollection<ProductOrderDetail> ProductOrderDetails { get; set; }
    }
}
