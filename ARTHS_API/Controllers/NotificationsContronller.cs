using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Post;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Json;
using ARTHS_Service.Interfaces;
using ARTHS_Data.Models.Views;
using ARTHS_API.Configurations.Middleware;
using ARTHS_Utility.Constants;
using Swashbuckle.AspNetCore.Annotations;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Put;

namespace ARTHS_API.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationsContronller : ControllerBase
    {

        private readonly INotificationService _notificationService;

        public NotificationsContronller(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        [HttpGet]
        [Authorize(UserRole.Teller, UserRole.Customer, UserRole.Owner)]
        [ProducesResponseType(typeof(ListViewModel<NotificationViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all notifications of logged in account.")]
        public async Task<ActionResult<ListViewModel<NotificationViewModel>>> GetNotifications([FromQuery] PaginationRequestModel pagination)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            return await _notificationService.GetNotifications(auth!.Id, pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(NotificationViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get notification by id.")]
        public async Task<ActionResult<NotificationViewModel>> GetNotification([FromRoute] Guid Id)
        {
            return await _notificationService.GetNotification(Id);
        }


        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(NotificationViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update notification.")]
        public async Task<IActionResult> UpdateNotification([FromRoute] Guid Id, [FromBody] UpdateNotificationModel model)
        {
            var notification = await _notificationService.UpdateNotification(Id, model);
            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Delete notification.")]
        public async Task<ActionResult<bool>> DeleteNotification([FromRoute] Guid Id)
        {
            var notification = await _notificationService.DeleteNotification(Id);
            return notification ? NoContent() : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(NotificationViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Send notification.")]
        public async Task<ActionResult<string>> SendNotification([FromBody] List<string> deviceToken)
        {
            var messageData = new Dictionary<string, string>
                    {
                        { "type", "type_ne" },
                        { "link", "link_ne" },
                        { "createAt", DateTime.Now.ToString() }
                    };
            var message = new MulticastMessage()
            {
                Notification = new FirebaseAdmin.Messaging.Notification()
                {
                    Title = "Test thoông báo",
                    Body = "Test thông báo."
                },
                Data = messageData,
                Tokens = deviceToken
            };
            var app = FirebaseApp.DefaultInstance;
            if (FirebaseApp.DefaultInstance == null)
            {
                GoogleCredential credential;
                var credentialJson = Environment.GetEnvironmentVariable("GoogleCloudCredential");
                if (string.IsNullOrWhiteSpace(credentialJson))
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
            return "Send thành công";
        }
    }
}

