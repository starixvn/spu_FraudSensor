namespace Fanex.NAP.FS.Websites.Controllers
{
    using Library.Services;
    using System.Linq;
    using System.Web.Mvc;
    
    public class NotificationController : ConfigurationsController
    {
        private readonly INotificationService _notificationService;

        public NotificationController()
        {
            _notificationService = new NotificationService();
        }

        [HttpGet]
        public JsonResult GetAllMessages()
        {
            var userId = CurrentSession.UserId;
            var messages = new System.Collections.Generic.List<Library.DTO.Message>();//_notificationService.Get(userId).ToList();
            messages.Add(new Library.DTO.Message { FraudCaseName = "New message", IsViewed = false, LogDate = System.DateTime.Now, MessageContent = "temp content" });
            messages.Add(new Library.DTO.Message { FraudCaseName = "Old message", IsViewed = true, LogDate = System.DateTime.Now, MessageContent = "temp content is reviewed" });

            return Json(messages, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ReCheckNotification()
        {
            var userId = CurrentSession.UserId;
            var unreadMessageCounter = 1;// _notificationService.GetUnreadMessageCounter(userId);

            return Json(new { UnreadMessageCounter = unreadMessageCounter }, JsonRequestBehavior.AllowGet);
        }
    }
}