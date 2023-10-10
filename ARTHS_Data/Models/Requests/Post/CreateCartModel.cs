namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateCartModel
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
