namespace ARTHS_Data.Models.Requests.Post
{
    public class RegisterOwnerModel
    {
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
    }
}
