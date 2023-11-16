using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationViewModel> GetNotification(Guid id);
        Task<ListViewModel<NotificationViewModel>> GetNotifications(Guid accountId, PaginationRequestModel pagination);
        Task<bool> SendNotification(ICollection<Guid> accountIds, CreateNotificationModel model);
        Task<NotificationViewModel> UpdateNotification(Guid id, UpdateNotificationModel model);
        Task<bool> DeleteNotification(Guid Id);

        Task CheckAndSendMaintenanceReminders();
    }
}
