namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Fanex.NAP.FS.Library.ViewModels.AssociatedAccountMonitor;
    using System.Text;
    using System.Web.Mvc;

    public class AssociatedAccountsMonitorController : BaseController
    {
        private readonly IAssociatedAccountsMonitorService _service;

        public AssociatedAccountsMonitorController()
        {
            _service = new AssociatedAccountsMonitorService();
        }        

        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult Index()
        {
            var evidencesCache = new StringBuilder();
            var evidences = _service.GetAllEvidences();

            foreach (var evidence in evidences)
            {
                evidencesCache.Append(evidence.EvidenceCode + ":" + evidence.EvidenceName + ";");
            }

            ViewBag.Evidences = evidences;
            ViewBag.EvidencesCache = evidencesCache.ToString();
            ViewBag.Subscribers = _service.GetAllSubscribers();
            ViewBag.Username = this.User.Identity.Name;

            return View();
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult GetAssociatedAccountsMonitor(AssociatedAccountsMonitorFilter filter)
        {
            var data = _service.GetAssociatedAccountsMonitor(filter);
            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult GetAllAssociatedAccounts(AllAssociatedAccountsFilter filter)
        {
            var data = _service.GetAllAssociatedAccounts(filter);
            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult AssociatedAccountsMonitorDetail(AssociatedAccountsMonitorDetailFilter filter)
        {
            var evidencesCache = new StringBuilder();
            var evidences = _service.GetAllEvidences();

            foreach (var evidence in evidences)
            {
                evidencesCache.Append(evidence.EvidenceCode + ":" + evidence.EvidenceName + ";");
            }

            ViewBag.Evidences = evidences;
            ViewBag.EvidencesCache = evidencesCache.ToString();
            ViewBag.Subscribers = _service.GetAllSubscribers();
            ViewBag.AccountCustId = filter.AccountCustId;
            ViewBag.Username = filter.Username;
            ViewBag.NoOfAssociations = filter.NoOfAssociations;
            ViewBag.DateFrom = filter.DateFrom;
            ViewBag.DateTo = filter.DateTo;
            ViewBag.DateRange = filter.DateFrom.ToString("MMM dd, yyyy") + " - " + filter.DateTo.ToString("MMM dd, yyyy");

            return View("Detail");
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult GetAssociatedAccountsMonitorDetail(AssociatedAccountsMonitorDetailFilter filter)
        {
            var data = _service.GetAssociatedAccountsMonitorDetail(filter);
            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult VerifyAssociatedAccountMonitor(AssociatedAccountMonitorVerify accountVerify)
        {
            accountVerify.UserId = CurrentSession.UserId;
            var data = _service.VerifyAssociatedAccountMonitor(accountVerify);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociatedAccountsMonitor, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult UpdateNoteAssociatedAccountMonitor(AssociatedAccountMonitorNote accountNote)
        {
            var data = _service.UpdateNoteAssociatedAccountMonitor(accountNote);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}