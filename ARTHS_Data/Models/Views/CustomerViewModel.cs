using ARTHS_Data.Entities;

namespace ARTHS_Data.Models.Views
{
    public class CustomerViewModel
    {
        public Guid Id { get; set; }
        public string Address { get; set; } = null!;

        public virtual AccountViewModel Account { get; set; } = null!;
    }
}
