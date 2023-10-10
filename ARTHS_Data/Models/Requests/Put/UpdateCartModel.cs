using ARTHS_Data.Models.Requests.Post;

namespace ARTHS_Data.Models.Requests.Put
{
    public class UpdateCartModel
    {
        public List<CreateCartModel> CartItems { get; set; } = new List<CreateCartModel>();
    }
}
