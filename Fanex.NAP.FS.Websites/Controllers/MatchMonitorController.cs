namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Service.Impl;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Criteria.MatchMonitor;
    using Fanex.NAP.FS.Library.DTO;
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

        private readonly bool _isShowSpecialCurrencies;

        private const string SessionCustomerList = "CustomerList";

        public MatchMonitorController()
        {
            _fraudCaseDetailService = new FraudCaseDetailsService(int.Parse(ConfigurationManager.AppSettings["Environment"]), int.Parse(ConfigurationManager.AppSettings["MailGroupId"]));
            _matchMonitorService = new MatchMonitorService();
            _isShowSpecialCurrencies = new UserService().GetPermission(CurrentSession.UserId, SiteId, FSFunctionKeys.SpecialCurrencies) > 0;
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
            filter.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
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
            filter.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var templatePath = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/template.xlsx");
            var fileName = "MatchMonitor_" + DateTime.Now.ToString(Consts.EXPORTEXCEL_DATETIME_FORMAT) + ".xlsx";
            var excelFileContent = _matchMonitorService.ExportExcel(filter, templatePath);
            string fileId = Guid.NewGuid().ToString();
            TempData[fileId] = excelFileContent;

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { FileId = fileId, FileName = fileName } };
        }

        public ActionResult DownloadExcel(string fileId, string fileName)
        {
            if (TempData[fileId] != null)
            {
                byte[] excelFileContent = TempData[fileId] as byte[];
                TempData[fileId] = null;

                return File(excelFileContent, "application/vnd.ms-excel", fileName);
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
            return Json(_fraudCaseDetailService.GetLast7DaysDetails(custId, _isShowSpecialCurrencies));
        }

        public ActionResult GetAssociation(int custId)
        {
            return Json(_fraudCaseDetailService.GetAssociatedAccounts(custId));
        }

        public ActionResult GetTicketFromIP(string ip)
        {
            return Json(_fraudCaseDetailService.GetTicketsFromIP(ip, _isShowSpecialCurrencies));
        }

        public ActionResult GetExcelTicketFromIP(string ip)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/ticket by ip.xlsx");
            var fileName = string.Format("TicketsofIP_{0}_{1}", ip, DateTime.Today.ToString(Consts.EXPORTEXCEL_DATETIME_FORMAT));

            return ExcelFile(_fraudCaseDetailService.ExportExcelTicketsByIP(ip, template, _isShowSpecialCurrencies), fileName);
        }

        #endregion Common

        #region Other Fraud Cases
        public ActionResult GetFraudCaseDetails(int fraudCaseId, int matchId, DateTime eventDate, int violations)
        {
            var model = _fraudCaseDetailService.GetMatchInfo(matchId, fraudCaseId, eventDate);
            ViewBag.BUDate = _matchMonitorService.GetBalanceUpDate().BalanceUpDay;

            if (fraudCaseId == (int)Library.Models.FraudCaseId.Arbitrage
                || fraudCaseId == (int)Library.Models.FraudCaseId.FlaggedAccount)
            {
                ViewBag.SiteList = _fraudCaseDetailService.GetSites();
                ViewBag.CurrencyList = _fraudCaseDetailService.GetAllCurrency(_isShowSpecialCurrencies);

                var evidences = _fraudCaseDetailService.GetListEvidences();
                ViewBag.EvidenceList = evidences;
                ViewBag.OtherEvidenceCodes = evidences.Where(e => e.IsOther).Select(e => e.EvidenceCode).Join();
            }

            return View("FraudCaseDetails/Index", model);
        }

        public ActionResult GetFraudCaseTicketDetails(GetTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;

            var fraudTicketsDetails =
                _fraudCaseDetailService.GetTicketDetails(criteria)
                .GroupBy(t => t.Bettype)
                .Select(g => new { key = g.Key, data = g.AsEnumerable() });

            return Json(fraudTicketsDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFraudCaseCustomerDetails(GetCustomerCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var fraudCustomersDetails = _fraudCaseDetailService.GetCustomerDetails(criteria);

            return Json(fraudCustomersDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportExcelForDetail(GetTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/template.xlsx");
            var fileName = string.Format("FraudCaseDetails_{0}.xlsx", DateTime.Today.ToString(Consts.EXPORTEXCEL_DATETIME_FORMAT));

            return File(_fraudCaseDetailService.ExportExcelForDetails(criteria, template), "application/vnd.ms-excel", fileName);
        }

        #endregion Other Fraud Cases

        #region Flagged Accounts

        [HttpPost]
        public ActionResult GetFlaggedAccountTickets(GetFlaggedAccountTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var tickets =
               _fraudCaseDetailService.GetFlaggedAccountTickets(criteria)
               .GroupBy(t => t.Bettype)
               .Select(g => new { key = g.Key, data = g.AsEnumerable() });

            return Json(tickets, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetFlaggedAccountCustomers(GetFlaggedAccountCustomerCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var customers = _fraudCaseDetailService.GetFlaggedAccountCustomers(criteria);
        
            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTotalAssociationCustomers(string custIds)
        {
            var custIdList = custIds != null ? custIds.Split(',').Select(Int32.Parse).ToList() : new List<int>();
            var associations = _fraudCaseDetailService.GetTotalAssociations(custIdList);            

            return Json(associations, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveFileFlaggedAccounts(GetFlaggedAccountTicketCriteria criteria)
        {
            var template = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/flagged account.xlsx");
            var fileName = string.Format("FraudCaseDetails_{0}.xlsx", DateTime.Today.ToString(Consts.EXPORTEXCEL_DATETIME_FORMAT));
            criteria.SaveChoice = true;
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var excelFileContent = _fraudCaseDetailService.ExportExcelFlaggedAccounts(criteria, template);
            string fileId = Guid.NewGuid().ToString();
            TempData[fileId] = excelFileContent;

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { FileId = fileId, FileName = fileName } };
        }

        public ActionResult GetBetTypes()
        {
            var data = _fraudCaseDetailService.GetBetTypes();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Arbitrage        
        [HttpPost]
        public ActionResult GetArbitrageTickets(GetArbitrageTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var data = _fraudCaseDetailService.GetArbitrageTickets(criteria, false);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetArbitrageTicketsGroupSummary(GetArbitrageTicketCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var aribitrageTickets = _fraudCaseDetailService.GetArbitrageTickets(criteria, true).ToList();

            var custIdList = new List<long>();
            foreach (var item in aribitrageTickets.Select(a => a.Home.Select(t => t.CustId)))
            {
                custIdList.AddRange(item);
            }

            foreach (var item in aribitrageTickets.Select(a => a.Away.Select(t => t.CustId)))
            {
                custIdList.AddRange(item);
            }

            SessionManager.Set(SessionCustomerList, custIdList.Distinct().ToList());

            return new JsonResult { Data = new { Content = aribitrageTickets }, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost]
        public JsonResult GetFraudCaseArbitrageCustomerDetails(GetArbitrageCustomerCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            if (criteria.IsOnlyCN88)
            {
                criteria.Site = "CN88";
            }
            
            var custIdList = new List<long>();
            if (SessionManager.Get<List<long>>(SessionCustomerList) != null)
            {
                custIdList = SessionManager.Get<List<long>>(SessionCustomerList);
            }
            var arbitrageCustomerDetails = _fraudCaseDetailService.GetArbitrageCustomerDetails(criteria, custIdList.Count);

            if (arbitrageCustomerDetails is List<CustomerDetailsForMuchData>)
            {
                var arbitrageCustomerDetailsMuchData = arbitrageCustomerDetails as List<CustomerDetailsForMuchData>;
                var totalCustomersCombineWithTickets = arbitrageCustomerDetailsMuchData[0].CustIdList.Join(custIdList.AsEnumerable(),
                            custA => custA,
                            custB => custB,
                            (custA, custB) => custA).Count();
                arbitrageCustomerDetailsMuchData[0].TotalCustomers = totalCustomersCombineWithTickets;

                return Json(arbitrageCustomerDetailsMuchData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                arbitrageCustomerDetails = arbitrageCustomerDetails.Join(custIdList.AsEnumerable(),
                            custA => custA.CustId,
                            custB => custB,
                            (custA, custB) => custA);
            }

            return Json(arbitrageCustomerDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveFileArbitrageTickets(GetArbitrageTicketCriteria criteria)
        {
            var templatePath = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/arbitrage.xlsx");
            var fileName = string.Format("FraudCaseDetails_{0}", DateTime.Today.ToString(Consts.EXPORTEXCEL_DATETIME_FORMAT)) + ".xlsx";
            string[] filterInfo = GenerateFilterInfo(criteria);
            bool isSummary = criteria.GroupId == 0;
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var excelFileContent = _fraudCaseDetailService.ExportExcelArbitrageTickets(criteria, filterInfo, templatePath, isSummary);
            string fileId = Guid.NewGuid().ToString();
            TempData[fileId] = excelFileContent;

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { FileId = fileId, FileName = fileName } };
        }

        [HttpPost]
        public ActionResult SaveFileArbitrageCustomers(GetArbitrageCustomerCriteria criteria)
        {
            var custIdList = new List<long>();
            if (SessionManager.Get<List<long>>(SessionCustomerList) != null)
            {
                custIdList = SessionManager.Get<List<long>>(SessionCustomerList);
            }

            var templatePath = Server.MapPath("~/App_Data/Excel Template/MatchMonitor/FraudCaseDetail/arbitrage.xlsx");
            var fileName = string.Format("FraudCaseDetails_{0}", DateTime.Today.ToString(Consts.EXPORTEXCEL_DATETIME_FORMAT)) + ".xlsx";
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            string[] filterInfo = GenerateFilterInfo(criteria);
            var excelFileContent = _fraudCaseDetailService.ExportExcelArbitrageCustomers(criteria, filterInfo, templatePath, custIdList);
            string fileId = Guid.NewGuid().ToString();
            TempData[fileId] = excelFileContent;

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
            var otherEvidenceCode = criteria.OtherEvidenceCodes;
            if (otherEvidenceCode != null && criteria.EvidenceCodeList != null)
            {
                strCode = criteria.EvidenceCodeList;
                strCode = strCode.Replace(otherEvidenceCode, "Others");
                strCode = strCode.Replace(',', '/');
            }

            filterInfo[2] = _fraudCaseDetailService.ReplaceFilterEvidences(strCode);

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

        public JsonResult GetScoreHDP(GetScoreHDPCriteria criteria)
        {
            criteria.IsShowSpecialCurrencies = _isShowSpecialCurrencies;
            var data = _fraudCaseDetailService.GetScoreHDP(criteria);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion Arbitrage       

    }
}