using Microsoft.AspNetCore.Http;

namespace ARTHS_Data.Models.Requests.Put
{
    public class UpdateRepairServiceModel
    {
        public string? Name { get; set; }
        public int? Price { get; set; }
        public int? Duration { get; set; }
        public int? ReminderInterval { get; set; }
        public int? WarrantyDuration { get; set; }
        public Guid? DiscountId { get; set; }

        public string? Description { get; set; }
        public string? Status { get; set; }



        //public List<IFormFile>? Images { get; set; } = new List<IFormFile>();
    }
}
