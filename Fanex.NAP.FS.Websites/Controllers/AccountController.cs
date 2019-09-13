namespace Fanex.NAP.FS.Websites.Controllers
{
    using System.Net;
    using System.Web.Mvc;
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

            var filter = GenerateGeneralInformationFilter(id, custId);

            var data = _accountService.GetAccountDetailGeneralInformation(filter);

            return View("DetailAccount", data);
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

        private GeneralInformationFilter GenerateGeneralInformationFilter(int id, int custId)
        {
            return new GeneralInformationFilter
            {
                AccountId = id,
                CustId = custId,
                IsMasterSubscriberAssignment = PermissionService.IsMasterSubscriberAssignment(),
                UserId = CurrentSession.UserId
            };
        }        
        #endregion
    }
}