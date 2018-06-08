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
            messages.Add(new Library.DTO.Message { FraudCaseName = "Daily Data Monitoring", IsViewed = false, LogDateString = System.DateTime.Now.ToString("MMM dd, yyyy H:mm"), MessageContent = "There are 22 matches which having data changes on Sep 22, 2018 06:06." });
            messages.Add(new Library.DTO.Message { FraudCaseName = "Check Bet", IsViewed = true, LogDateString = System.DateTime.Now.ToString("MMM dd, yyyy H:mm"), MessageContent = "There are 229 tickets which having data changes on Sep 22, 2018 06:06." });

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