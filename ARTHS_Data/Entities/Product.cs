using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class Product
    {
        public Product()
        {
            CartItems = new HashSet<CartItem>();
            FeedbackProducts = new HashSet<FeedbackProduct>();
            ProductImages = new HashSet<ProductImage>();
            ProductOrderDetails = new HashSet<ProductOrderDetail>();
            ProductPrices = new HashSet<ProductPrice>();
            RepairOrderDetails = new HashSet<RepairOrderDetail>();
            Categories = new HashSet<Category>();
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid Id { get; set; }
        public Guid? RepairServiceId { get; set; }
        public Guid? DiscountId { get; set; }
        public Guid? WarrantyId { get; set; }
        public string Name { get; set; } = null!;
        public int PriceCurrent { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? UpdateAt { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual Discount? Discount { get; set; }
        public virtual RepairService? RepairService { get; set; }
        public virtual Warranty? Warranty { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<FeedbackProduct> FeedbackProducts { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductOrderDetail> ProductOrderDetails { get; set; }
        public virtual ICollection<ProductPrice> ProductPrices { get; set; }
        public virtual ICollection<RepairOrderDetail> RepairOrderDetails { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
