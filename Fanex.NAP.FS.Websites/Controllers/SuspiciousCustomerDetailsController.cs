namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.FS.Library.Services;
    using Library.ViewModels.SuspiciousCustomerDetails;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class SuspiciousCustomerDetailsController : ReportsController
    {
        private readonly ISuspiciousCustomerReportDetailsService _detailsService = new SuspiciousCustomerReportDetailsService();

        public ActionResult Index(int custId, string custName, DateTime from, DateTime to)
        {
            var generalViewModel = GenerateViewModel(custId, custName, from, to);

            return View(generalViewModel);
        }

        public ActionResult UpdateAssignee(int custId, int assigneeId)
        {
            var result = _detailsService.UpdateAssignee(custId, assigneeId);

            return Json(result);
        }

        public ActionResult UpdateCustomerFraudCaseVerify(int custId, int fraudCaseId)
        {
            var result = _detailsService.VerifySuspiciousCustomerFraudCase(custId, CurrentSession.UserId, fraudCaseId);

            return Json(result);
        }

        public ActionResult RemoveLogNote(int custId, int customerLogId)
        {
            var result = _detailsService.DeleteCustomerLogNotes(custId, customerLogId);

            return Json(result);
        }

        public ActionResult InsertLogNote(int custId, string logDesc)
        {
            var result = _detailsService.InsertCustomerLogNotes(custId, CurrentSession.UserId, logDesc);

            return Json(result);
        }

        public ActionResult UpdateCustomerInfo(int custId, bool isDelinquent, bool isDirty, bool isHedge)
        {
            var result = _detailsService.UpdateSuspiciousCustomerInfo(custId, CurrentSession.UserId, isDelinquent, isDirty, isHedge);

            return Json(result);
        }

        public async Task<ActionResult> GetCustomerLogNotes(int custId)
        {
            return Json(_detailsService.GetCustomerLogNotes(custId));
        }

        public async Task<ActionResult> GetStatistics(int custId, DateTime from, DateTime to)
        {
            return Json(_detailsService.GetStatistics(custId, from, to));
        }

        public async Task<ActionResult> GetViolatedTickets(int custId, DateTime from, DateTime to)
        {
            return JData(_detailsService.GetViolatedTickets(custId, from, to));
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

            generalViewModel.CustInfo = _detailsService.GetSuspiciousCustomerInfo(custId);
            generalViewModel.Assignee = _detailsService.GetAssignee(custId);
            generalViewModel.ViolatedFraudCases = _detailsService.GetSuspiciousCustomerFraudCase(custId);

            return generalViewModel;
        }

        #endregion
    }
}