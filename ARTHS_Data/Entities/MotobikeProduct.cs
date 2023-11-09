using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class MotobikeProduct
    {
        public MotobikeProduct()
        {
            CartItems = new HashSet<CartItem>();
            FeedbackProducts = new HashSet<FeedbackProduct>();
            Images = new HashSet<Image>();
            MotobikeProductPrices = new HashSet<MotobikeProductPrice>();
            OrderDetails = new HashSet<OrderDetail>();
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid Id { get; set; }
        public Guid? DiscountId { get; set; }
        public Guid? WarrantyId { get; set; }
        public Guid? CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public int PriceCurrent { get; set; }
        public int InstallationFee { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? UpdateAt { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Discount? Discount { get; set; }
        public virtual Warranty? Warranty { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<FeedbackProduct> FeedbackProducts { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<MotobikeProductPrice> MotobikeProductPrices { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
