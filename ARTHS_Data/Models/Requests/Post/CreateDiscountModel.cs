using Microsoft.AspNetCore.Http;

namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateDiscountModel
    {
        public string Title { get; set; } = null!;
        public int DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IFormFile Image { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<Guid>? MotobikeProductId { get; set; } = new List<Guid>();
        public List<Guid>? RepairServiceId { get; set; } = new List<Guid>();
    }
}
