namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Service.Impl;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Criteria.MatchMonitor;
    using Library.Core.Helpers;
    using Library.Criteria.MatchMonitor.FraudCaseDetails;
    using Library.Models.MatchMonitor.FraudCaseDetails;
    using Library.Service;
    using Service;
    using Service.MatchMonitor;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Text;
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
            ViewBag.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);

            return View(_matchMonitorService.GetBalanceUpDate());
        }

        #region Common
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

        #endregion Common

        #region Other Fraud Cases
        public ActionResult GetFraudCaseDetails(int fraudCaseId, int matchId, DateTime eventDate, int violations)
        {
            var model = _fraudCaseDetailService.GetInfo(matchId, fraudCaseId, eventDate);

            ViewBag.BUDate = _matchMonitorService.GetBalanceUpDate().BalanceUpDay;
            ViewBag.EvidenceList = _fraudCaseDetailService.GetListEvidences();

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

        #endregion Other Fraud Cases

        #region Arbitrage
        public ActionResult GetListArbitrageTickets(GetArbitrageTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var data = _fraudCaseDetailService.GetListArbitrageTickets(criteria);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetArbitrageTicketsGroupSummary(GetArbitrageTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var data = _fraudCaseDetailService.GetArbitrageTickets(criteria);
            return new JsonResult { Data = new { Content = data }, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult ExportCSVArbitrageTickets(GetArbitrageTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;

            string[] filterInfo = GenerateFilterInfoForCsv(criteria);
            var content = _fraudCaseDetailService.ExportArbitrageTicketAllCsv(criteria, filterInfo);
            var encoded = Encoding.UTF8.GetBytes(content);

            return File(encoded, "text/plain", "FraudCaseDetails_Tickets_" + DateTime.Today.ToString("yyyyMMdd") + ".csv");
        }

        [HttpPost]
        public JsonResult GetFraudCaseArbitrageCustomerDetails(GetArbitrageCustomerCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var data = _fraudCaseDetailService.GetArbitrageCustomerDetails(criteria);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportCSVArbitrageCustomers(GetArbitrageCustomerCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;

            string[] filterInfo = GenerateFilterInfoForCsv(criteria);
            var content = _fraudCaseDetailService.ExportArbitrageCustomersCsv(criteria, filterInfo);
            var encoded = Encoding.UTF8.GetBytes(content);

            return File(encoded, "text/plain", "FraudCaseDetails_Customers_" + DateTime.Today.ToString("yyyyMMdd") + ".csv");
        }

        public ActionResult ExportExcelForDetail(int fraudCaseId, int matchId, DateTime eventDate)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/template.xlsx");

            var fileName = string.Format("FraudCaseDetails_{0}.xlsx", DateTime.Today.ToString("yyyyMMdd"));
            return File(_fraudCaseDetailService.ExportExcel(matchId, fraudCaseId, eventDate, template, IsShowSpecialCurrencies), "application/vnd.ms-excel", fileName);
        }

        public ActionResult ExportCSVArbitrageTicketByGroup(GetArbitrageTicketCriteria criteria, int groupId, string score, decimal handicap)
        {
            string[] filterInfo = GenerateFilterInfoForCsv(criteria);

            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var content = _fraudCaseDetailService.ExportArbitrageTicketByGroupCsv(criteria, groupId, score, filterInfo);
            var encoded = Encoding.UTF8.GetBytes(content);

            return File(encoded, "text/plain", "FraudCaseDetails_Tickets_" + DateTime.Today.ToString("yyyyMMdd") + ".csv");
        }

        private string[] GenerateFilterInfoForCsv(ArbitrageDetailsCriteria criteria)
        {
            string[] filterInfo = new string[6];

            // get Market
            filterInfo[0] = criteria.LiveStatus == -1 ? "All" : (criteria.LiveStatus == 0 ? "Non-Live" : "Live");

            // get Currency
            var obj = HttpContext.Cache["ListCurrency"];
            if (obj != null)
            {
                IEnumerable<Currency> lstObject = obj as IEnumerable<Currency>;
                Currency currency = lstObject.FirstOrDefault(c => c.CurrencyId == criteria.CurrencyId.ToString());
                filterInfo[1] = currency != null ? currency.CurrencyName : "All";
            }

            // get Evidence name
            string strCode = string.Empty;
            var otherEvidenceCode = AppSettings.Get("FPS_GET_OTHER_EVIDENCES");
            if (otherEvidenceCode != null && criteria.EvidenceCodeList != null)
            {
                strCode = criteria.EvidenceCodeList;
                strCode = strCode.Replace(otherEvidenceCode, "Others");
                strCode = strCode.Replace(',', '/');
            }

            filterInfo[2] = strCode;

            // From date, to date
            filterInfo[3] = criteria.TransDateFromString != null ? criteria.TransDateFromString.Replace(',', '-') : string.Empty;
            filterInfo[4] = criteria.TransDateToString != null ? criteria.TransDateToString.Replace(',', '-') : string.Empty;

            // get Score/HDP
            filterInfo[5] = criteria.KeyName;

            return filterInfo;
        }

        public JsonResult GetListEvidences()
        {
            var data = _fraudCaseDetailService.GetListEvidences();
            HttpContext.Cache["ListEvidences"] = data;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllCurrency()
        {
            var data = _fraudCaseDetailService.GetAllCurrency(IsShowSpecialCurrencies);
            HttpContext.Cache["ListCurrency"] = data;

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetScoreHDP(int matchId, DateTime eventDate)
        {
            var data = _fraudCaseDetailService.GetScoreHDP(matchId, IsShowSpecialCurrencies);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion Arbitrage
    }
}