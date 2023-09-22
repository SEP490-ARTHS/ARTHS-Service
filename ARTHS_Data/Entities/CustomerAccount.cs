using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class CustomerAccount
    {
        public CustomerAccount()
        {
            FeedbackProducts = new HashSet<FeedbackProduct>();
            FeedbackStaffs = new HashSet<FeedbackStaff>();
            ProductOrders = new HashSet<ProductOrder>();
        }

        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Address { get; set; } = null!;

        public virtual Account Account { get; set; } = null!;
        public virtual Cart? Cart { get; set; }
        public virtual ICollection<FeedbackProduct> FeedbackProducts { get; set; }
        public virtual ICollection<FeedbackStaff> FeedbackStaffs { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}
