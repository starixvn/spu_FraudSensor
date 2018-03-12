using Fanex.NAP.Core.Web.Attribute;
using Fanex.NAP.FS.Websites.Criteria.MatchMonitor;
using Fanex.NAP.FS.Websites.Service.MatchMonitor;
using System;
using System.Web.Mvc;

namespace Fanex.NAP.FS.Websites.Controllers
{
    [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Read)]
    public class MatchMonitorController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Username = this.User.Identity.Name;

            return View(MatchMonitorService.GetBalanceUpDate());
        }

        [HttpPost]
        public ActionResult GetMatches(MatchGetAllCriteria criteria)
        {
            var matchList = MatchMonitorService.GetMatches(criteria);

            return Json(new { data = matchList.Matches }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetLeagues(LeagueGetCriteria criteria)
        {
            return Json(MatchMonitorService.GetLeagues(criteria));
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Host)]
        public ActionResult VerifyMatch(VerifyMatchCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;

            return Json(MatchMonitorService.VerifyMatch(criteria));
        }

        public ActionResult ExportExcel(MatchGetAllCriteria criteria)
        {
            var templatePath = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/template.xlsx");

            var fileName = DateTime.Now.ToString("MMM-d-yyyy") + ".xlsx";

            return File(MatchMonitorService.ExportExcel(criteria, templatePath), "application/vnd.ms-excel", fileName);
        }

        [HttpPost]
        public ActionResult UpdateMatchNote(MatchNoteUpdateCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;

            return Json(MatchMonitorService.UpdateMatchNote(criteria));
        }
    }
}