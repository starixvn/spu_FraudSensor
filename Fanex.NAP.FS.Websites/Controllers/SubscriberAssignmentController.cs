namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.Core.Web.Session;
    using Fanex.NAP.FS.Library.Services;
    using System;
    using System.Configuration;
    using System.Web.Mvc;

    public class SubscriberAssignmentController : BaseController
    {
        private readonly ISubscriberAssignmentService _subscriberAssignmentService;
        private bool IsShowSpecialCurrencies;

        public SubscriberAssignmentController()
        {
            _subscriberAssignmentService = new SubscriberAssignmentService();

            var permission = new UserService().GetPermission(CurrentSession.UserId, SiteId, FSFunctionKeys.SpecialCurrencies);
            IsShowSpecialCurrencies = permission > 0;
        }
     
        public ActionResult Index()
        {
            ViewBag.Username = this.User.Identity.Name;
            ViewBag.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);

            return View();
        }

        public ActionResult GetSubscribers(string userId)
        {
            var data = _subscriberAssignmentService.GetSubscribers(userId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}