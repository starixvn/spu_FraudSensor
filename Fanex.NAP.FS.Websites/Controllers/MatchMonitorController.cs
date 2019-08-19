namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Service.Impl;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Criteria.MatchMonitor;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor;
    using Fanex.NAP.FS.Library.ViewModels.MatchMonitor.FraudCaseDetails;
    using Fanex.NAP.FS.Service.Utilities;
    using Library.Core.Helpers;
    using Library.Criteria.MatchMonitor.FraudCaseDetails;
    using Library.Service;
    using Service;
    using Service.MatchMonitor;
    using System;
    using System.Collections.Generic;
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
            ViewBag.Environment = int.Parse(ConfigurationManager.AppSettings["Environment"]);
            ViewBag.Sites = _fraudCaseDetailService.GetSites();

            return View(_matchMonitorService.GetBalanceUpDate());
        }

        #region Common
        [HttpPost]
        public ActionResult GetMatches(MatchFilter filter)
        {
            filter.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var matchList = _matchMonitorService.GetMatches(filter);

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

        [HttpPost]
        public ActionResult GenerateExcel(MatchFilter filter)
        {
            filter.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var templatePath = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/template.xlsx");

            var fileName = "MatchMonitor_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

            var content = _matchMonitorService.ExportExcel(filter, templatePath);

            string fileId = Guid.NewGuid().ToString();

            TempData[fileId] = content;

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { FileId = fileId, FileName = fileName } };
        }

        public ActionResult DownloadExcel(string fileId, string fileName)
        {
            if (TempData[fileId] != null)
            {
                byte[] content = TempData[fileId] as byte[];

                TempData[fileId] = null;

                return File(content, "application/vnd.ms-excel", fileName);
            }

            return new EmptyResult();
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

            if (fraudCaseId == (int)Library.Models.FraudCaseId.Arbitrage
                || fraudCaseId == (int)Library.Models.FraudCaseId.FlaggedAccount)
            {
                ViewBag.SiteList = _fraudCaseDetailService.GetSites();
                ViewBag.CurrencyList = _fraudCaseDetailService.GetAllCurrency(IsShowSpecialCurrencies);
                ViewBag.EvidenceList = _fraudCaseDetailService.GetListEvidences();
            }

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

        public ActionResult ExportExcelForDetail(int fraudCaseId, int matchId, DateTime eventDate)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/template.xlsx");

            var fileName = string.Format("FraudCaseDetails_{0}.xlsx", DateTime.Today.ToString("yyyyMMdd"));
            return File(_fraudCaseDetailService.ExportExcel(matchId, fraudCaseId, eventDate, template, IsShowSpecialCurrencies), "application/vnd.ms-excel", fileName);
        }

        #endregion Other Fraud Cases

        #region Flagged Accounts

        [HttpPost]
        public ActionResult GetFlaggedAccountTickets(GetFlaggedAccountTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;

            var data =
               _fraudCaseDetailService.GetFlaggedAccountTickets(criteria)
               .GroupBy(t => t.Bettype)
               .Select(g => new { key = g.Key, data = g.AsEnumerable() });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetFlaggedAccountCustomers(GetFlaggedAccountCustomerCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;

            var data = _fraudCaseDetailService.GetFlaggedAccountCustomers(criteria);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveFileFlaggedAccounts(GetFlaggedAccountTicketCriteria criteria)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/flagged account.xlsx");
            var fileName = string.Format("FraudCaseDetails_{0}.xlsx", DateTime.Today.ToString("yyyyMMdd"));

            criteria.SaveChoice = true;
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var content = _fraudCaseDetailService.ExportFlaggedAccounts(criteria, template);

            string fileId = Guid.NewGuid().ToString();
            TempData[fileId] = content;

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { FileId = fileId, FileName = fileName } };
        }

        public ActionResult GetBetType()
        {
            var data = _fraudCaseDetailService.GetBetTypes();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Arbitrage        
        [HttpPost]
        public ActionResult GetArbitrageTickets(GetArbitrageTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var data = _fraudCaseDetailService.GetArbitrageTickets(criteria, false);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetArbitrageTicketsGroupSummary(GetArbitrageTicketCriteria criteria)
        {
            var data = new List<ArbitrageGroupTicketList>();
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            data = _fraudCaseDetailService.GetArbitrageTickets(criteria, true).ToList();

            var lstCustId = new List<long>();
            foreach (var item in data.Select(a => a.Home.Select(t => t.CustId)))
            {
                lstCustId.AddRange(item);
            }

            foreach (var item in data.Select(a => a.Away.Select(t => t.CustId)))
            {
                lstCustId.AddRange(item);
            }

            SessionManager.Set("ListCustId", lstCustId.Distinct().ToList());

            return new JsonResult { Data = new { Content = data }, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost]
        public JsonResult GetFraudCaseArbitrageCustomerDetails(GetArbitrageCustomerCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            if (criteria.IsOnlyCN88)
            {
                criteria.Site = "CN88";
            }

            var data = _fraudCaseDetailService.GetArbitrageCustomerDetails(criteria);

            if (SessionManager.Get<List<long>>("ListCustId") != null)
            {
                var lstCustId = SessionManager.Get<List<long>>("ListCustId");
                data = data.Where(a => lstCustId.Contains(a.CustId));
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveFileArbitrageTickets(GetArbitrageTicketCriteria criteria)
        {
            var templatePath = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/arbitrage.xlsx");
            var fileName = string.Format("FraudCaseDetails_{0}", DateTime.Today.ToString("yyyyMMdd")) + ".xlsx";
            string[] filterInfo = GenerateFilterInfo(criteria);

            bool isSummary = criteria.GroupId == 0;
            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            var content = _fraudCaseDetailService.ExportExcelArbitrageTickets(criteria, filterInfo, templatePath, isSummary);

            string fileId = Guid.NewGuid().ToString();

            TempData[fileId] = content;

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { FileId = fileId, FileName = fileName } };
        }

        [HttpPost]
        public ActionResult SaveFileArbitrageCustomers(GetArbitrageCustomerCriteria criteria)
        {
            List<long> lstCustId = null;
            if (SessionManager.Get<List<long>>("ListCustId") != null && criteria.IsByIPAndDevice)
            {
                lstCustId = SessionManager.Get<List<long>>("ListCustId");
            }

            var templatePath = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/arbitrage.xlsx");

            var fileName = string.Format("FraudCaseDetails_{0}", DateTime.Today.ToString("yyyyMMdd")) + ".xlsx";

            criteria.IsShowSpecialCurrencies = IsShowSpecialCurrencies;
            string[] filterInfo = GenerateFilterInfo(criteria);

            var content = _fraudCaseDetailService.ExportExcelArbitrageCustomers(criteria, filterInfo, templatePath, lstCustId);
            string fileId = Guid.NewGuid().ToString();

            TempData[fileId] = content;

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { FileId = fileId, FileName = fileName } };
        }

        private string[] GenerateFilterInfo(ArbitrageDetailsCriteria criteria)
        {
            string[] filterInfo = new string[10];

            // get Market
            filterInfo[0] = criteria.LiveStatus == -1 ? "All" : (criteria.LiveStatus == 0 ? "Non-Live" : "Live");

            // get Currency
            filterInfo[1] = criteria.CurrencyNames;

            // get Evidence name
            string strCode = string.Empty;
            var otherEvidenceCode = AppSettings.Get("FPS_GET_OTHER_EVIDENCES");
            if (otherEvidenceCode != null && criteria.EvidenceCodeList != null)
            {
                strCode = criteria.EvidenceCodeList;
                strCode = strCode.Replace(otherEvidenceCode, "Others");
                strCode = strCode.Replace(',', '/');
            }

            filterInfo[2] = _fraudCaseDetailService.ReplaceEvidences(strCode);

            // From date, to date
            filterInfo[3] = criteria.TransDateFromString != null ? criteria.TransDateFromString.Replace(',', '-') : string.Empty;
            filterInfo[4] = criteria.TransDateToString != null ? criteria.TransDateToString.Replace(',', '-') : string.Empty;

            // get Score/HDP
            filterInfo[5] = criteria.KeyName;

            // Sites
            filterInfo[6] = criteria.Sites;

            filterInfo[7] = criteria.Odds == null ? string.Empty : criteria.Odds.ToString();
            filterInfo[8] = criteria.IsOnlyCN88 ? "Yes" : "No";
            filterInfo[9] = criteria.IsByIPAndDevice ? "Yes" : "No";

            return filterInfo;
        }

        public JsonResult GetScoreHDP(int matchId, DateTime eventDate)
        {
            var data = _fraudCaseDetailService.GetScoreHDP(matchId, IsShowSpecialCurrencies);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion Arbitrage       

    }
}