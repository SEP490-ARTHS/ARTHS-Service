using Microsoft.AspNetCore.Http;

namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateRepairServiceModel
    {
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public int Duration { get; set; }
        public int? ReminderInterval { get; set; }
        public string Description { get; set; } = null!;
        public int WarrantyDuration { get; set; }
        public Guid? DiscountId { get; set; }

        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
