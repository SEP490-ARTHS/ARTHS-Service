using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Enums;
using ARTHS_Utility.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class NotificationService : BaseService, INotificationService
    {
        private readonly IDeviceTokenRepository _deviceTokenRepository;
        private readonly INotificationRepository _notificationRepository;

        private readonly IMaintenanceScheduleRepository _maintenanceScheduleRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _deviceTokenRepository = unitOfWork.DeviceToken;
            _notificationRepository = unitOfWork.Notification;
            _maintenanceScheduleRepository = unitOfWork.MaintenanceSchedule;
            _orderDetailRepository = unitOfWork.OrderDetail;
        }

        public async Task<NotificationViewModel> GetNotification(Guid id)
        {
            return await _notificationRepository.GetMany(notification => notification.Id.Equals(id))
                .ProjectTo<NotificationViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy thông báo.");
        }

        public async Task<ListViewModel<NotificationViewModel>> GetNotifications(Guid accountId, PaginationRequestModel pagination)
        {
            var query = _notificationRepository.GetMany(noti => noti.AccountId.Equals(accountId));
            var notifications = await query
                .OrderByDescending(noti => noti.SendDate)
                .ProjectTo<NotificationViewModel>(_mapper.ConfigurationProvider)
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize)
                .AsNoTracking()
                .ToListAsync();
            var totalRow = await query.AsNoTracking().CountAsync();
            return notifications != null ? new ListViewModel<NotificationViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = notifications
            } : null!;
        }

        public async Task<bool> SendNotification(ICollection<Guid> accountIds, CreateNotificationModel model)
        {
            var deviceTokens = await _deviceTokenRepository.GetMany(token => accountIds.Contains(token.AccountId))
                .Select(token => token.Token)
                .ToListAsync();
            var now = DateTime.UtcNow.AddHours(7);
            foreach (var accountId in accountIds)
            {
                var notification = new ARTHS_Data.Entities.Notification
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    SendDate = now,
                    Body = model.Body,
                    Type = model.Data.Type,
                    Link = model.Data.Link,
                    Title = model.Title
                };
                _notificationRepository.Add(notification);
            }

            var result = await _unitOfWork.SaveChanges();
            if(result > 0)
            {
                if (deviceTokens.Any())
                {
                    var messageData = new Dictionary<string, string>
                    {
                        { "type", model.Data.Type ?? "" },
                        { "link", model.Data.Link ?? "" },
                        { "createAt", now.ToString() }
                    };
                    var message = new MulticastMessage()
                    {
                        Notification = new FirebaseAdmin.Messaging.Notification()
                        {
                            Title = model.Title,
                            Body = model.Body
                        },
                        Data = messageData,
                        Tokens = deviceTokens
                    };
                    var app = FirebaseApp.DefaultInstance;
                    if (FirebaseApp.DefaultInstance == null)
                    {
                        GoogleCredential credential;
                        var credentialJson = Environment.GetEnvironmentVariable("GoogleCloudCredential");
                        if(string.IsNullOrWhiteSpace(credentialJson))
                        {
                            var basePath = AppDomain.CurrentDomain.BaseDirectory;
                            var projectRoot = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", ".."));
                            string credentialPath = Path.Combine(projectRoot, "ARTHS_Utility", "Helpers", "CloudStorage", "arths-45678-firebase-adminsdk-plwhs-954089d6b7.json");
                            credential = GoogleCredential.FromFile(credentialPath);
                        }
                        else
                        {
                            credential = GoogleCredential.FromJson(credentialJson);
                        }

                        app = FirebaseApp.Create(new AppOptions()
                        {
                            Credential = credential
                        });
                    }
                    FirebaseMessaging messaging = FirebaseMessaging.GetMessaging(app);
                    await messaging.SendMulticastAsync(message);
                }
            }
            return true;
        }

        public async Task<NotificationViewModel> UpdateNotification(Guid id, UpdateNotificationModel model)
        {
            var notification = await _notificationRepository.GetMany(notification => notification.Id.Equals(id)).FirstOrDefaultAsync();
            if (notification == null)
            {
                return null!;
            }
            notification.IsRead = model.IsRead;
            _notificationRepository.Update(notification);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetNotification(id) : null!;
        }

        public async Task<bool> MakeAsRead(Guid accountId)
        {
            var notifications = await _notificationRepository.GetMany(notification => notification.AccountId.Equals(accountId)).ToListAsync();
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }
            _notificationRepository.UpdateRange(notifications);
            var result = await _unitOfWork.SaveChanges();
            return result > 0;
        }

        public async Task<bool> DeleteNotification(Guid Id)
        {
            var notification = await _notificationRepository.GetMany(noti => noti.Id.Equals(Id)).FirstOrDefaultAsync();
            if (notification == null) return false;
             _notificationRepository.Remove(notification);
            var result = await _unitOfWork.SaveChanges();
            return result > 0;
        }

        public async Task CheckAndSendMaintenanceReminders()
        {
            var today = DateTime.UtcNow;
            var maintenanceSchedules = await _maintenanceScheduleRepository
                .GetMany(m => m.ReminderDate.Date == today.Date && !m.RemiderSend)
                .ToListAsync();

            if (maintenanceSchedules.Count == 0) return;

            foreach (var schedule in maintenanceSchedules)
            {
                await SendNotificationToCustomer(schedule);
                schedule.RemiderSend = true;
            }

            _maintenanceScheduleRepository.UpdateRange(maintenanceSchedules);
            await _unitOfWork.SaveChanges();
        }

        private async Task SendNotificationToCustomer(MaintenanceSchedule schedule)
        {
            var detail = await _orderDetailRepository.GetMany(detail => detail.Id.Equals(schedule.OrderDetailId))
                .Include(detail => detail.RepairService)
                .FirstOrDefaultAsync();

            var message = new CreateNotificationModel
            {
                Title = $"Nhắc nhở sắp đến lịch bảo trỉ tiếp theo.",
                Body = $"Bạn đã sử dụng dịch vụ bảo trì bảo dưỡng {detail!.RepairService!.Name} " +
                $"bên chúng tôi và đã sắp đến hạn bảo dưỡng lần tiếp theo vào ngày {schedule.NextMaintenanceDate.ToString("ddMMyyy")}. " +
                $"Để đảm bảo được tình trạng xe tốt nhất bạn nên đặt lịch sửa bảo trì lần tiếp theo hoặc có thể đem xe đến để chúng tôi có thể chăm sóc tốt cho xe của bạn.",
                Data = new NotificationDataViewModel
                {
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    Type = NotificationType.RepairService.ToString(),
                    Link = detail.Id.ToString()
                }
            };
            //var staffId = await _accountRepository.GetMany(account => account.Id.Equals(order.StaffId)).Select(account => account.Id).FirstOrDefaultAsync();
            await SendNotification(new List<Guid> { schedule.CustomerId }, message);
        }
    }
}
