using Microsoft.AspNetCore.Http;

namespace ARTHS_Data.Models.Requests.Put
{
    public class UpdateRepairServiceModel
    {
        public string? Name { get; set; }
        public int? Price { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        public List<IFormFile>? Images { get; set; } = new List<IFormFile>();
    }
}
