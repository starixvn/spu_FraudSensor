using Fanex.NAP.Core.Web.Attribute;
using Fanex.NAP.FS.Websites.Criteria.MatchMonitor;
using Fanex.NAP.FS.Websites.Service.MatchMonitor;
using System;
using System.Linq;
using System.Net.Http;
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

        public ActionResult GetFraudCaseDetails(int fraudCaseId, int matchId, DateTime eventDate)
        {
            var model = FraudCaseDetailsService.GetInfo(matchId, fraudCaseId, eventDate);

            return View("FraudCaseDetails/Index", model);
        }

        public ActionResult GetFraudCaseTicketDetails(int fraudCaseId, int matchId, DateTime eventDate)
        {
            var data = 
                FraudCaseDetailsService.GetTicketDetails(matchId, fraudCaseId, eventDate)
                .GroupBy(t => t.Bettype)
                .Select(g => new { key = g.Key, data = g.AsEnumerable() });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFraudCaseCustomerDetails(int fraudCaseId, int matchId, DateTime eventDate)
        {
            var data = FraudCaseDetailsService.GetCustomerDetails(matchId, fraudCaseId, eventDate);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportExcelForDetail(int fraudCaseId, int matchId, DateTime eventDate)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/template.xlsx");

            var fileName = string.Format("FraudCaseDetails_{0}.xlsx", DateTime.Today.ToString("yyyyMMdd"));
            return File(FraudCaseDetailsService.ExportExcel(matchId, fraudCaseId, eventDate, template), "application/vnd.ms-excel", fileName);
        }

        [HttpPost]
        public ActionResult UpdateMatchNote(MatchNoteUpdateCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;
            criteria.Notes = criteria.Notes ?? string.Empty;

            return Json(MatchMonitorService.UpdateMatchNote(criteria));
        }

        public ActionResult GetLast7DaysTicket(int custId)
        {
            return Json(FraudCaseDetailsService.GetLast7DaysDetails(custId));
        }

        public ActionResult GetAssociation(int custId)
        {
            return Json(FraudCaseDetailsService.GetAssociatedAccounts(custId));
        }
    }
}