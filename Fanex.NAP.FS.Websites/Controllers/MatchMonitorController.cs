#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Criteria.MatchMonitor;
    using Library.Service;
    using Service;
    using Service.MatchMonitor;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.MatchMonitoring, Policy = Core.Business.Constant.Policy.Read)]
    public class MatchMonitorController : ReportsController
    {
        private readonly IFraudCaseDetailsService _fraudCaseDetailService;
        private readonly IMatchMonitorService _matchMonitorService;

        public MatchMonitorController()
        {
            _fraudCaseDetailService = new FraudCaseDetailsService(int.Parse(ConfigurationManager.AppSettings["Environment"]), int.Parse(ConfigurationManager.AppSettings["MailGroupId"]));
            _matchMonitorService = new MatchMonitorService();
        }

        public ActionResult Index()
        {
            ViewBag.Username = this.User.Identity.Name;

            return View(_matchMonitorService.GetBalanceUpDate());
        }

        [HttpPost]
        public ActionResult GetMatches(MatchGetAllCriteria criteria)
        {
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

        public async Task<ActionResult> GetFraudCaseTicketDetails(int fraudCaseId, int matchId, DateTime eventDate)
        {
            var data =
                _fraudCaseDetailService.GetTicketDetails(matchId, fraudCaseId, eventDate)
                .GroupBy(t => t.Bettype)
                .Select(g => new { key = g.Key, data = g.AsEnumerable() });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetFraudCaseCustomerDetails(int fraudCaseId, int matchId, DateTime eventDate)
        {
            var data = _fraudCaseDetailService.GetCustomerDetails(matchId, fraudCaseId, eventDate);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportExcelForDetail(int fraudCaseId, int matchId, DateTime eventDate)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/template.xlsx");

            var fileName = string.Format("FraudCaseDetails_{0}.xlsx", DateTime.Today.ToString("yyyyMMdd"));
            return File(_fraudCaseDetailService.ExportExcel(matchId, fraudCaseId, eventDate, template), "application/vnd.ms-excel", fileName);
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
            return Json(_fraudCaseDetailService.GetLast7DaysDetails(custId));
        }

        public ActionResult GetAssociation(int custId)
        {
            return Json(_fraudCaseDetailService.GetAssociatedAccounts(custId));
        }

        public ActionResult GetTicketFromIP(string ip)
        {
            return Json(_fraudCaseDetailService.GetTicketFromIP(ip));
        }

        public ActionResult GetExcelTicketFromIP(string ip)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/ticket by ip.xlsx");

            var fileName = string.Format("TicketsofIP_{0}_{1}", ip, DateTime.Today.ToString("yyyyMMdd"));

            return ExcelFile(_fraudCaseDetailService.ExportToExcelIpTickets(ip, template), fileName);
        }
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
