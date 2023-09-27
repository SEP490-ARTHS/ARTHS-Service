using ARTHS_Data.Entities;

namespace ARTHS_Data.Models.Views
{
    public class CustomerViewModel
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Avatar { get; set; }

        public virtual CartViewModel? Cart { get; set; }
        
    }
}
