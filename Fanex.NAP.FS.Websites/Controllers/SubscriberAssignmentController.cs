namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.Core.Web.Session;
    using Fanex.NAP.FS.Library.Services;
    using System;
    using System.Configuration;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.SubscriberAssignment, Policy = Core.Business.Constant.Policy.Read)]
    public class SubscriberAssignmentController : BaseController
    {
        private readonly ISubscriberAssignmentService _subscriberAssignmentService;      

        public SubscriberAssignmentController()
        {
            _subscriberAssignmentService = new SubscriberAssignmentService();
        }
     
        public ActionResult Index()
        {
            ViewBag.Username = this.User.Identity.Name;
            ViewBag.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);

            return View();
        }

        public ActionResult GetUserId(string userName)
        {
            var userService = new UserService();

            var user = userService.Get(userName);

            if(user != null && userService.HasAnyPermission(user.Id, SiteId, Environment))
            {
                return Json(user.Id, JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubscribers(string userId)
        {
            var data = _subscriberAssignmentService.GetSubscribers(userId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}