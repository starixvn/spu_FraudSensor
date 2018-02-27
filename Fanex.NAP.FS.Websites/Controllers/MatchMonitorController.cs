using Fanex.NAP.Core.Web.Attribute;
using Fanex.NAP.FS.Websites.Criteria.MatchMonitor;
using Fanex.NAP.FS.Websites.Service.MatchMonitor;
using System.Web.Mvc;

namespace Fanex.NAP.FS.Websites.Controllers
{
    [NapSuperAuth]
    public class MatchMonitorController : BaseController
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
        public ActionResult GetLeagues(LeagueGetCriteria criteria)
        {
            return Json(MatchMonitorService.GetLeagues(criteria));
        }

        [HttpPost]
        public ActionResult VerifyMatch(VerifyMatchCriteria criteria)
        {
            criteria.Verifier = CurrentSession.UserId;

            return Json(MatchMonitorService.VerifyMatch(criteria));
        }
    }
}