#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Fanex.NAP.FS.Websites.Controllers
{
    using Core.Service.Impl;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails;
    using Fanex.NAP.FS.Library.Services;
    using Library.ViewModels.SuspiciousCustomerDetails;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.SuspiciousCustomerDetails, Policy = Core.Business.Constant.Policy.Read)]
    public class SuspiciousCustomerDetailsController : ReportsController
    {
        private readonly ISuspiciousCustomerReportDetailsService _detailsService = new SuspiciousCustomerReportDetailsService();

        private bool IsShowSpecialCurrencies;

        public SuspiciousCustomerDetailsController()
        {
            var permission = new UserService().GetPermission(CurrentSession.UserId, SiteId, FSFunctionKeys.SpecialCurrencies);
            IsShowSpecialCurrencies = permission > 0;
        }

        public ActionResult Index(int custId, string custName, DateTime from, DateTime to)
        {
            var generalViewModel = GenerateViewModel(custId, custName, from, to);

            return View(generalViewModel);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.SuspiciousCustomerDetails, Policy = Core.Business.Constant.Policy.Host)]
        public ActionResult UpdateAssignee(int custId, int? assigneeId)
        {
            var result = _detailsService.UpdateAssignee(custId, assigneeId, CurrentSession.UserId);

            return Json(result);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.SuspiciousCustomerDetails, Policy = Core.Business.Constant.Policy.Host)]
        public ActionResult UpdateCustomerFraudCaseVerify(int custId, int fraudCaseId)
        {
            var result = _detailsService.VerifySuspiciousCustomerFraudCase(custId, CurrentSession.UserId, fraudCaseId);

            return Json(result);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.SuspiciousCustomerDetails, Policy = Core.Business.Constant.Policy.Host)]
        public ActionResult RemoveLogNote(int custId, int customerLogId)
        {
            var result = _detailsService.DeleteCustomerLogNotes(custId, customerLogId);

            return Json(result);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.SuspiciousCustomerDetails, Policy = Core.Business.Constant.Policy.Host)]
        public ActionResult InsertLogNote(int custId, string logDesc)
        {
            var result = _detailsService.InsertCustomerLogNotes(custId, CurrentSession.UserId, logDesc);

            return Json(result);
        }

        [NapAuth(FunctionKey = FSFunctionKeys.SuspiciousCustomerDetails, Policy = Core.Business.Constant.Policy.Host)]
        public ActionResult UpdateCustomerInfo(int custId, bool? isDelinquent, bool? isDirty, bool? isHedge)
        {
            var result = _detailsService.UpdateSuspiciousCustomerInfo(custId, CurrentSession.UserId, isDelinquent, isDirty, isHedge);

            return Json(result);
        }

        public ActionResult GetCustomerLogNotes(int custId)
        {
            return JData(_detailsService.GetCustomerLogNotes(custId));
        }

        public ActionResult GetStatistics(int custId, DateTime from, DateTime to)
        {
            return Json(_detailsService.GetStatistics(custId, from, to, IsShowSpecialCurrencies));
        }

        public ActionResult GetViolatedTickets(int custId, DateTime from, DateTime to)
        {
            return JData(_detailsService.GetViolatedTickets(custId, from, to, IsShowSpecialCurrencies));
        }

        public ActionResult GetFraudCases(int custId)
        {
            return JData(_detailsService.GetSuspiciousCustomerFraudCase(custId));
        }

        #region Utilities

        private CustomerDetailsViewModel GenerateViewModel(int custId, string custName, DateTime from, DateTime to)
        {
            var generalViewModel = new CustomerDetailsViewModel
            {
                Username = custName,
                FromDate = from,
                ToDate = to
            };

            generalViewModel.CustInfo = _detailsService.GetSuspiciousCustomerInfo(custId, IsShowSpecialCurrencies);
            generalViewModel.Assignee = _detailsService.GetAssignee(custId);
            generalViewModel.Currency = _detailsService.GetCustomerCurrency(custId);

            return generalViewModel;
        }

        #endregion Utilities
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously