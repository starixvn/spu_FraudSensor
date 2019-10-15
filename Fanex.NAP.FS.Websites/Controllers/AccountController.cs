namespace Fanex.NAP.FS.Websites.Controllers
{
    using System.Net;
    using System.Web.Mvc;
    using System.Linq;
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.Services;
    using Fanex.NAP.FS.Library.ViewModels.Account.Associated;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.ViewModels.Evidence;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.FS.Library.ViewModels.Account.GeneralInformation;
    using Fanex.NAP.FS.Service.Utilities;
    using Fanex.NAP.FS.Library.ViewModels.Account.Evidence;
    using Fanex.NAP.FS.Library.ViewModels.Account.Evidence.Exception;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Core.Resources;

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
        public ActionResult DetailAccount(int id, int custId)
        {
            var permissionIPLocation = new UserService().GetPermission(CurrentSession.UserId, SiteId, FSFunctionKeys.IPLocation);

            ViewBag.IsPermissionIPLocation = permissionIPLocation > 0;
            ViewBag.AccountId = id;
            ViewBag.CustId = custId;         
            return View("DetailAccount");
        }

        [HttpPost]
        public ActionResult GetGeneralInformation(GeneralInformationFilter filter)
        {
            filter.IsMasterSubscriberAssignment = PermissionService.IsMasterSubscriberAssignment();
            filter.UserId = CurrentSession.UserId;

            var data = _accountService.GetAccountDetailGeneralInformation(filter);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetChainsToEvidence(ChainsToEvidenceFilter filter)
        {
            filter.UserId = CurrentSession.UserId;

            filter.IsMasterSubscriberAssignment = PermissionService.IsMasterSubscriberAssignment();

            var data = _evidenceService.GetChainsToEvidence(filter);

            return PartialView("DetailInfoAccount/ChainsToEvidence", data);
        }

        [HttpPost]
        public ActionResult AddExceptionEvidence(ExceptionEvidenceAdd exceptionEvidenceAdd)
        {
            exceptionEvidenceAdd.UserId = CurrentSession.UserId;

            var data = _evidenceService.AddExceptionEvidence(exceptionEvidenceAdd);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetRetractAndException(RetractedAndExceptionEvidenceFilter filter)
        {
            filter.UserId = CurrentSession.UserId;

            filter.IsMasterSubscriberAssignment = PermissionService.IsMasterSubscriberAssignment();

            var data = _evidenceService.GetRetractedAndExceptionEvidence(filter);

            return PartialView("DetailInfoAccount/RetractAndException", data);
        }

        [HttpPost]
        public ActionResult GetAssociatedAccountByAccount()
        {
            ViewBag.AccountStatusList = GetAccountStatusList();

            return PartialView("Associated/AccountByAccount");
        }

        [HttpPost]
        public ActionResult GetAssociatedAccountByAccountFilter(AssociatedAccountByAccountFilter filter)
        {
            var msgError = string.Empty;

            filter.userId = CurrentSession.UserId;

            filter.isMasterSubscriberAssignment = PermissionService.IsMasterSubscriberAssignment();

            var data = _accountService.GetAssociatedAccountByAccounts(filter, out msgError);

            if (!string.IsNullOrEmpty(msgError))
            {
                return ResponseError(msgError);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetShortestPath(int startCustId, int endCustId)
        {
            var data = _accountService.GetShortestPath(startCustId, endCustId);
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
        public ActionResult PlaceEvidenceByAccount(EvidenceByAccount evidenceByAccount)
        {
            var data = _evidenceService.PlaceEvidenceByAccount(evidenceByAccount);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private ActionResult ResponseError(string msgError)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var errors = new List<string>();
            errors.Add(msgError);

            return Json(errors, JsonRequestBehavior.AllowGet);
        }

        private static Dictionary<AccountStatus, string> GetAccountStatusList()
        {
            var listStatus = new Dictionary<AccountStatus, string>();
            listStatus.Add(AccountStatus.Open, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_OPEN));
            listStatus.Add(AccountStatus.Suspended, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_SUSPENDED));
            listStatus.Add(AccountStatus.Closed, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CLOSED));

            if (PermissionService.IsPermissionCN88())
            {
                listStatus.Add(AccountStatus.CN88_Active, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CN88_NAME_ACTIVE));
                listStatus.Add(AccountStatus.CN88_ViewOnly, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CN88_NAME_VIEW_ONLY));
                listStatus.Add(AccountStatus.CN88_Suspended, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CN88_NAME_SUSPENDED));
                listStatus.Add(AccountStatus.CN88_Inactive, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CN88_NAME_INACTIVE));
            }

            listStatus.Add(AccountStatus.NotYetDeposit, ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_NOTYETDEPOSIT));

            return listStatus;
        }

        #endregion
    }
}