namespace Fanex.NAP.FS.Areas.UserLogin.Controllers
{
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using System.Web.Mvc;
    using System.Web.Security;

    /// <summary>
    /// Common Controller.
    /// </summary>
    public class CommonController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;

        public CommonController()
            : this(null)
        {
        }

        public CommonController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService ?? new MaintenanceService();
        }

        /// <summary>
        /// Off line function.
        /// </summary>
        /// <returns>Return result.</returns>
        public ActionResult Offline()
        {
            var downTime = _maintenanceService.GetUnderMaintenanceTime();
            
            if (!downTime.IsUnderMaintenanceTime)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            
            ViewBag.UmtFrom = downTime.From;
            ViewBag.UmtTo = downTime.To;

            return View();
        }

        public ActionResult AccessDenied()
        {
            return View("AccessDenied");
        }

        public ActionResult Error()
        {
            return View("Error");
        }
    }
}