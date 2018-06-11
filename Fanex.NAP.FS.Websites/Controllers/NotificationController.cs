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
            var messages = _notificationService.Get(userId).ToList();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ReCheckNotification()
        {
            var userId = CurrentSession.UserId;
            var unreadMessageCounter = _notificationService.GetUnreadMessageCounter(userId);

            return Json(new { UnreadMessageCounter = unreadMessageCounter }, JsonRequestBehavior.AllowGet);
        }
    }
}