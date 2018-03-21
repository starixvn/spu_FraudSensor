using Fanex.NAP.Core.Web.Attribute;
using Fanex.NAP.FS.Websites.Criteria.MatchMonitor;
using Fanex.NAP.FS.Websites.Models.FraudCase;
using Fanex.NAP.FS.Websites.Service.MatchMonitor;
using Fanex.NAP.FS.Websites.ViewModels.MatchMonitor.FraudCaseDetails;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Fanex.NAP.FS.Websites.Controllers
{
    [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Read)]
    public class MatchMonitorController : ReportsController
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

            var fileName ="MatchMonitor_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

            return File(MatchMonitorService.ExportExcel(criteria, templatePath), "application/vnd.ms-excel", fileName);
        }

        public ActionResult GetFraudCaseDetails(int fraudCaseId, int matchId)
        {
            var model = new MatchFraudCaseInfo
            {
                Event = "String vs Loop",
                FraudCase = "Gravity",
                HTScore = "0 - 0",
                FTScore = "0 - 0",
                FraudCaseId = fraudCaseId,
                MatchId =  matchId
            };

            return View("FraudCaseDetails/Index", model);
        }

        public ActionResult GetFraudCaseTicketDetails(int fraudCaseId, int matchId)
        {
            var data = 
                FraudCaseDetailsService.GetTicketDetails(matchId, fraudCaseId)
                .GroupBy(t => t.Bettype);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFraudCaseCustomerDetails(int fraudCaseId, int matchId)
        {
            var data = FraudCaseDetailsService.GetCustomerDetails(matchId, fraudCaseId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateMatchNote(MatchNoteUpdateCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;
            criteria.Notes = criteria.Notes ?? string.Empty;

            return Json(MatchMonitorService.UpdateMatchNote(criteria));
        }
    }
}