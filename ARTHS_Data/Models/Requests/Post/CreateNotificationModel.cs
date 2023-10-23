using ARTHS_Data.Models.Views;

namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateNotificationModel
    {
        public ICollection<Guid> UserIds { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Body { get; set; } = null!;

        public NotificationDataViewModel Data { get; set; } = null!;
    }
}
