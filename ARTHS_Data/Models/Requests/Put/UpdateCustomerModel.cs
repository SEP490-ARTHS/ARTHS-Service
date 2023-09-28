namespace ARTHS_Data.Models.Requests.Put
{
    public class UpdateCustomerModel
    {
        //public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }

        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
