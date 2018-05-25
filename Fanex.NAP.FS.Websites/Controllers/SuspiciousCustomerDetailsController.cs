#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Fanex.NAP.FS.Websites.Controllers
{
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

        public async Task<ActionResult> GetCustomerLogNotes(int custId)
        {
            return JData(_detailsService.GetCustomerLogNotes(custId));
        }

        public async Task<ActionResult> GetStatistics(int custId, DateTime from, DateTime to)
        {
            var key1 = $"{custId} - {from} - {to} - data";

            var key2 = $"{custId} - {from} - {to} - counter";

            if (Session[key1] == null)
            {
                Session[key1] = Task.Run(() => _detailsService.GetStatistics(custId, from, to));
                Session[key2] = 0;
            }

            var tsk = Session[key1] as Task<Statistics>;

            var result = await tsk.ConfigureAwait(false);

            var counter = (int)Session[key2];

            Session[key2] = counter + 1;

            if ((int)Session[key2] >= 4)
            {
                Session.Remove(key1);
                Session.Remove(key2);
            }

            return Json(result);
        }

        public async Task<ActionResult> GetViolatedTickets(int custId, DateTime from, DateTime to)
        {
            return JData(_detailsService.GetViolatedTickets(custId, from, to));
        }

        public async Task<ActionResult> GetFraudCases(int custId)
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

            generalViewModel.CustInfo = _detailsService.GetSuspiciousCustomerInfo(custId);
            generalViewModel.Assignee = _detailsService.GetAssignee(custId);

            return generalViewModel;
        }

        #endregion Utilities
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously