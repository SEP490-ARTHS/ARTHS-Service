using ARTHS_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTHS_Data.Models.Views
{
    public class InStoreOrderViewModel
    {
        public string Id { get; set; } = null!;
        public string TellerName { get; set; } = null!;
        public string? StaffName { get; set; }
        public string? CustomerName { get; set; }
        public string CustomerPhone { get; set; } = null!;
        public string? LicensePlate { get; set; }
        public string Status { get; set; } = null!;
        public string? PaymentMethod { get; set; }
        public int TotalAmount { get; set; }
        public string OrderType { get; set; } = null!;
        public DateTime OrderDate { get; set; }

        //public virtual StaffAccount Staff { get; set; } = null!;
        //public virtual TellerAccount Teller { get; set; } = null!;
        //public virtual Bill? Bill { get; set; }
        public virtual ICollection<InStoreOrderDetailViewModel> InStoreOrderDetails { get; set; } = new List<InStoreOrderDetailViewModel>();
    }
}
