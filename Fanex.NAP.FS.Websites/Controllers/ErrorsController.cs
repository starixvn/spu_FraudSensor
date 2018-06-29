namespace Fanex.NAP.FS.Websites.Controllers
{
    using System.Web.Mvc;

    [RoutePrefix("errors")]
    public class ErrorsController : Controller
    {
        [Route("404")]
        public ActionResult Error404()
        {
            return View("404");
        }

        [Route("403")]
        public ActionResult Error403()
        {
            return View("403");
        }

        [Route("500")]
        public ActionResult InternalError()
        {
            return View("500");
        }

        public ActionResult Um()
        {
            return View("UM");
        }
    }
}