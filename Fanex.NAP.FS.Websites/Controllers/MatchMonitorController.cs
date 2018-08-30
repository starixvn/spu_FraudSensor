#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Service.Impl;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Criteria.MatchMonitor;
    using Fanex.NAP.FS.Library.Models;
    using Library.Service;
    using Service;
    using Service.MatchMonitor;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Read)]
    public class MatchMonitorController : ReportsController
    {
        private readonly IFraudCaseDetailsService _fraudCaseDetailService;

        private readonly IMatchMonitorService _matchMonitorService;

        private bool IsShowSpecialCurrencies;

        public MatchMonitorController()
        {
            _fraudCaseDetailService = new FraudCaseDetailsService(int.Parse(ConfigurationManager.AppSettings["Environment"]), int.Parse(ConfigurationManager.AppSettings["MailGroupId"]));
            _matchMonitorService = new MatchMonitorService();
            var permission = new UserService().GetPermission(CurrentSession.UserId, SiteId, FSFunctionKeys.SpecialCurrencies);
            IsShowSpecialCurrencies = permission > 0;
        }

        public ActionResult Index()
        {
            ViewBag.Username = this.User.Identity.Name;

            return View(_matchMonitorService.GetBalanceUpDate());
        }

        [HttpPost]
        public ActionResult GetMatches(MatchGetAllCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var matchList = _matchMonitorService.GetMatches(criteria);

            return Json(new { data = matchList.Matches }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetLeagues(LeagueGetCriteria criteria)
        {
            return Json(_matchMonitorService.GetLeagues(criteria));
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Host)]
        public ActionResult VerifyMatch(VerifyMatchCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;

            return Json(_matchMonitorService.VerifyMatch(criteria));
        }

        public ActionResult ExportExcel(MatchGetAllCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var templatePath = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/template.xlsx");

            var fileName = "MatchMonitor_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

            return File(_matchMonitorService.ExportExcel(criteria, templatePath), "application/vnd.ms-excel", fileName);
        }

        public ActionResult GetFraudCaseDetails(int fraudCaseId, int matchId, DateTime eventDate)
        {
            var model = _fraudCaseDetailService.GetInfo(matchId, fraudCaseId, eventDate);

            ViewBag.BUDate = _matchMonitorService.GetBalanceUpDate().BalanceUpDay;

            return View("FraudCaseDetails/Index", model);
        }

        public ActionResult GetFraudCaseTicketDetails(int fraudCaseId, int matchId, DateTime eventDate)
        {
            var data =
                _fraudCaseDetailService.GetTicketDetails(matchId, fraudCaseId, eventDate, IsShowSpecialCurrencies)
                .GroupBy(t => t.Bettype)
                .Select(g => new { key = g.Key, data = g.AsEnumerable() });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFraudCaseCustomerDetails(int fraudCaseId, int matchId, DateTime eventDate)
        {
            var data = _fraudCaseDetailService.GetCustomerDetails(matchId, fraudCaseId, eventDate, IsShowSpecialCurrencies);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetArbitrageTicketDetails(int matchId, DateTime eventDate)
        {
            var data = _fraudCaseDetailService.GetArbitrageTickets(matchId, eventDate);

            return Json(data);
        }

        public ActionResult ExportExcelForDetail(int fraudCaseId, int matchId, DateTime eventDate)
        {
            if (fraudCaseId == (int)FraudCaseId.Arbitrage)
            {
                var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/arbitrage.xlsx");

                var fileName = string.Format("FraudCaseDetails_{0}.xlsx", DateTime.Today.ToString("yyyyMMdd"));
                return File(_fraudCaseDetailService.ExportArbitrageExcel(matchId, eventDate, template), "application/vnd.ms-excel", fileName);
            }
            else
            {
                var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/template.xlsx");

                var fileName = string.Format("FraudCaseDetails_{0}.xlsx", DateTime.Today.ToString("yyyyMMdd"));
            return File(_fraudCaseDetailService.ExportExcel(matchId, fraudCaseId, eventDate, template, IsShowSpecialCurrencies), "application/vnd.ms-excel", fileName);
            }
        }

        [HttpPost]
        public ActionResult UpdateMatchNote(MatchNoteUpdateCriteria criteria)
        {
            criteria.UserId = CurrentSession.UserId;
            criteria.Notes = criteria.Notes ?? string.Empty;

            return Json(_matchMonitorService.UpdateMatchNote(criteria));
        }

        public ActionResult GetLast7DaysTicket(int custId)
        {
            return Json(_fraudCaseDetailService.GetLast7DaysDetails(custId, IsShowSpecialCurrencies));
        }

        public ActionResult GetAssociation(int custId)
        {
            return Json(_fraudCaseDetailService.GetAssociatedAccounts(custId));
        }

        public ActionResult GetTicketFromIP(string ip)
        {
            return Json(_fraudCaseDetailService.GetTicketFromIP(ip, IsShowSpecialCurrencies));
        }

        public ActionResult GetExcelTicketFromIP(string ip)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/ticket by ip.xlsx");

            var fileName = string.Format("TicketsofIP_{0}_{1}", ip, DateTime.Today.ToString("yyyyMMdd"));

            return ExcelFile(_fraudCaseDetailService.ExportToExcelIpTickets(ip, template, IsShowSpecialCurrencies), fileName);
        }

        public ActionResult ExportArbitrageWithFilterExcel(int matchId, DateTime eventDate, string bettype, string score, double handicap)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/arbitrage.xlsx");

            var fileName = string.Format("FraudCaseDetails_{0}", DateTime.Today.ToString("yyyyMMdd"));

            var content = _fraudCaseDetailService.ExportArbitrageExcel(matchId, eventDate, template, obj => obj.BettypeName == bettype && obj.Score == score && (double)obj.Handicap == handicap);

            return ExcelFile(content, fileName);
        }
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously