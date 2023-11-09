namespace ARTHS_Utility.Helpers.Models
{
    public class GhnCreateOrderModel
    {
        public string OrderId { get; set; } = null!;
        public string Note { get; set; } = "Vui lòng vận chuyển cẩn thận";
        public string Content { get; set; } = null!;
        public int Weight { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
