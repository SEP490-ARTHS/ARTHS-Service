using Microsoft.AspNetCore.Http;

namespace ARTHS_Data.Models.Requests.Put
{
    public class UpdateMotobikeProductModel
    {
        public string? Name { get; set; }
        public int? PriceCurrent { get; set; }
        public int? Quantity { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int? InstallationFee { get; set; }
        public Guid? DiscountId { get; set; }
        public Guid? WarrantyId { get; set; }
        public Guid? CategoryId { get; set; }
        public List<Guid>? VehiclesId { get; set; } = new List<Guid>();
        //public List<IFormFile>? Images { get; set; } = new List<IFormFile>();
    }
}
