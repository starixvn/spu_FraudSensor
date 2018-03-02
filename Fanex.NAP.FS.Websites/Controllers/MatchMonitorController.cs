using Fanex.NAP.Core.Web.Attribute;
using Fanex.NAP.FS.Websites.Criteria.MatchMonitor;
using Fanex.NAP.FS.Websites.Service.MatchMonitor;
using System;
using System.Web.Mvc;

namespace Fanex.NAP.FS.Websites.Controllers
{
    [NapSuperAuth]
    public class MatchMonitorController : BaseController
    {
        // GET: MatchMonitor
        public ActionResult Index()
        {
            return View(MatchMonitorService.GetBalanceUpDate());
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

        public ActionResult ExportExcel(MatchGetAllCriteria criteria)
        {
            var templatePath = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/template.xlsx");

            var fileName = DateTime.Now.ToString("MMM-d-yyyy") + ".xlsx";

            return File(MatchMonitorService.ExportExcel(criteria, templatePath), "application/vnd.ms-excel", fileName);
        }
    }
}