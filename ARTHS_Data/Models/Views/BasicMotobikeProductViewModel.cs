namespace ARTHS_Data.Models.Views
{
    public class BasicMotobikeProductViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int PriceCurrent { get; set; }
        public string Image { get; set; } = null!;
    }
}
