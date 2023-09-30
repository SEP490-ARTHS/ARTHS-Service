namespace ARTHS_Data.Models.Requests.Post
{
    public class RegisterTellerModel
    {
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
    }
}
