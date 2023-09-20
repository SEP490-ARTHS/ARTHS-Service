namespace ARTHS_Data.Models.Views
{
    public class CustomerViewModel
    {
        public Guid Id { get; set; }
        //public Guid AccountId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
