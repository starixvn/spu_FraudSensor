namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.AssociatedAccountMonitor;
    using Fanex.NAP.FS.Websites.Controllers.ActionFilter;
    using System.Text;
    using System.Web.Mvc;

    public class AssociatedAccountsMonitorController : BaseController
    {
        private readonly IAssociatedAccountsMonitorService service;

        public AssociatedAccountsMonitorController()
        {
            service = new AssociatedAccountsMonitorService();
        }        

        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult Index()
        {
            var evidencesCache = new StringBuilder();
            var evidences = service.GetAllEvidences();

            foreach (var evidence in evidences)
            {
                evidencesCache.Append(evidence.EvidenceCode + ":" + evidence.EvidenceName + ";");
            }

            ViewBag.Evidences = evidences;
            ViewBag.EvidencesCache = evidencesCache.ToString();
            ViewBag.Subscribers = service.GetAllSubscribers();
            ViewBag.Username = this.User.Identity.Name;

            return View();
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.AssAccMonitor_Get, null, null)]
        public ActionResult GetAssociatedAccountsMonitor(AssociatedAccountsMonitorFilter filter)
        {
            var data = service.GetAssociatedAccountsToMonitor(filter);
            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]        
        public ActionResult AssociatedAccountsDetailMonitor(AssociatedAccountsDetailMonitorFilter filter)
        {
            var evidencesCache = new StringBuilder();
            var evidences = service.GetAllEvidences();

            foreach (var evidence in evidences)
            {
                evidencesCache.Append(evidence.EvidenceCode + ":" + evidence.EvidenceName + ";");
            }

            ViewBag.Evidences = evidences;
            ViewBag.EvidencesCache = evidencesCache.ToString();
            ViewBag.Subscribers = service.GetAllSubscribers();
            ViewBag.AccountCustId = filter.AccountCustId;
            ViewBag.Username = filter.Username;
            ViewBag.NoOfAssociations = filter.NoOfAssociations;
            ViewBag.DateFrom = filter.DateFrom;
            ViewBag.DateTo = filter.DateTo;
            ViewBag.DateRange = filter.DateFrom.ToString("MMM dd, yyyy") + " - " + filter.DateTo.ToString("MMM dd, yyyy");

            return View();
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.AssAccMonitor_Detail_Get, null, null)]
        public ActionResult GetAssociatedAccountsDetailMonitor(AssociatedAccountsDetailMonitorFilter filter)
        {
            var data = service.GetAssociatedAccountsDetailToMonitor(filter);
            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.AssAccMonitor_Verify, null, null)]
        public ActionResult VerifyAssociatedAccountMonitor(AssociatedAccountMonitorVerify accountVerify)
        {
            accountVerify.UserId = CurrentSession.UserId;
            var data = service.VerifyAssociatedAccountMonitor(accountVerify);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.AssAccMonitor_UpdateNote, null, null)]
        public ActionResult UpdateNoteAssociatedAccountMonitor(AssociatedAccountMonitorNote accountNote)
        {
            var data = service.UpdateNoteAssociatedAccountMonitor(accountNote);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}