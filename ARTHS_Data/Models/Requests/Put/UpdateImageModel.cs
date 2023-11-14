using Microsoft.AspNetCore.Http;

namespace ARTHS_Data.Models.Requests.Put
{
    public class UpdateImageModel
    {
        public IFormFile Image { get; set; } = null!;
    }
}
