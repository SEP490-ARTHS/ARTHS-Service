namespace ARTHS_Data.Models.Views
{
    public class WarrantyViewModel
    {
        public Guid Id { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; } = null!;
    }
}
