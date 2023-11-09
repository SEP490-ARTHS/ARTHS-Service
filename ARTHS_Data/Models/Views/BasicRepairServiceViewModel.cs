namespace ARTHS_Data.Models.Views
{
    public class BasicRepairServiceViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Duration { get; set; }
        public int Price { get; set; }
        public int DiscountAmount { get; set; }
        public string Image { get; set; } = null!;
    }
}
