namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.FS.Library.Services;
    using Fanex.NAP.FS.Library.ViewModels.Account.Associated;
    using System.Web.Mvc;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.ViewModels.Evidence;
    using System.Net;
    using System.Collections.Generic;
    using Fanex.NAP.Core.Service.Impl;

    [NapAuth(FunctionKey = FSFunctionKeys.SearchAccount, Policy = Core.Business.Constant.Policy.Read)]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IEvidenceService _evidenceService;

        public AccountController()
        {
            _accountService = new AccountService();
            _evidenceService = new EvidenceService();
        }

        public override string BaseControllerName { get => Consts.MenuItemSearch; }

        public ActionResult Index()
        {
            return View();
        }

        #region Detail
        public ActionResult DetailAccount(int id, int custId, string subscriber)
        {
            var permissionIPLocation = new UserService().GetPermission(CurrentSession.UserId, SiteId, FSFunctionKeys.IPLocation);
            
            var data = _accountService.GetAccountDetailGeneralInformation(id, custId);

            ViewBag.SubscriberName = subscriber;
            ViewBag.IsPermissionIPLocation = permissionIPLocation > 0;

            return View("DetailAccount", data);
        }

        [HttpPost]
        public ActionResult GetChainsToEvidence(int accountId)
        {
            var data = _evidenceService.GetChainsToEvidence(accountId);

            return PartialView("DetailInfoAccount/ChainsToEvidence", data);
        }

        [HttpPost]
        public ActionResult AddException(int accountId, int exceptionAccountId, string comment)
        {
            var data = "";

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetRetractAndException(int accountId)
        {
            var data = _evidenceService.GetRetractedEvidenceAndException(accountId);

            return PartialView("DetailInfoAccount/RetractAndException", data);
        }

        [HttpPost]
        public ActionResult GetAssociatedAccountByAccount()
        {
            return PartialView("Associated/AccountByAccount");
        }

        [HttpPost]
        public ActionResult GetAssociatedAccountByAccountFilter(AssociatedAccountByAccountFilter filter)
        {
            var msgError = string.Empty;

            var data = _accountService.GetAssociatedAccountByAccounts(filter, out msgError);

            if (!string.IsNullOrEmpty(msgError))
            {
                return ResponseError(msgError);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAssociatedDeviceByAccount()
        {
            return PartialView("Associated/DeviceByAccount");
        }

        [HttpPost]
        public ActionResult GetAssociatedDeviceByAccountFilter(AssociatedDeviceByAccountFilter filter)
        {
            var data = _accountService.GetAssociatedDevicesByAccount(filter);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllEvidences()
        {
            var data = _evidenceService.GetAllEvidences();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEvidencesByAccount(int accountId)
        {
            var data = _evidenceService.GetEvidencesByAccount(accountId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }        

        [HttpPost]
        public ActionResult UpdateEvidenceByAccount(EvidenceByAccount evidenceByAccount)
        {
            var data = _evidenceService.UpdateEvidenceByAccount(evidenceByAccount);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private ActionResult ResponseError(string msgError)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var errors = new List<string>();
            errors.Add(msgError);

            return Json(errors, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}