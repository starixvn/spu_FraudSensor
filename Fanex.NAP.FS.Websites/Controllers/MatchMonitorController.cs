using Fanex.NAP.Core.Web.Attribute;
using Fanex.NAP.FS.Websites.Criteria.MatchMonitor;
using Fanex.NAP.FS.Websites.Service.MatchMonitor;
using System.Web.Mvc;

namespace Fanex.NAP.FS.Websites.Controllers
{
    [NapSuperAuth]
    public class MatchMonitorController : Controller
    {
        // GET: MatchMonitor
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetMatches(MatchGetAllCriteria criteria)
        {
            return Json(MatchMonitorService.GetMatches(criteria));
        }

        [HttpPost]
        public ActionResult VerifyMatch()
        {
            return null;
        }
    }
}