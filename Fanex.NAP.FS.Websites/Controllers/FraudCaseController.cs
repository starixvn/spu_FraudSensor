namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Core.Helpers;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.ExcelConfig.FraudCase.ScannedOfficeIP;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Services.Implement;
    using Fanex.NAP.FS.Library.Services.Interface;
    using Queue;
    using Library.Core.Helpers.Xml;
    using Library.Criteria.FraudCase.DelayedGoal;
    using Library.DTO;
    using Library.Services;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;
    using ViewModels.FraudCase;
    using Model;
    using EasyNetQ;
    using Library.Core.Utilities;
    using System.Threading.Tasks;
    using Fanex.NAP.FS.Websites.Controllers.ActionFilter;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.Logging;

    [NapAuth(FunctionKey = FSFunctionKeys.FraudCase, Policy = Core.Business.Constant.Policy.Read)]
    public class FraudCaseController : ConfigurationsController
    {
        private readonly string FPS_WEBAPI_GET_ASSOCIATION_ACCOUNT = ConfigurationManager.AppSettings["FPS_WEBAPI_GET_ASSOCIATION_ACCOUNT"];
        private readonly IDelayGoalAccountService _delayGoalAccountService;
        private readonly IFraudCaseService _fraudCaseService;
        private readonly IOfficeIPService _ipService;
        private readonly INotificationRecipientService _recipientService = new NotificationRecipientService();
        private static EasyNetQ _rabbitMQ;
        private static int envConfig = int.Parse(ConfigurationManager.AppSettings["Environment"]);

        public FraudCaseController()
        {
            _delayGoalAccountService = new DelayGoalAccountService();
            _fraudCaseService = new FraudCaseService();
            _ipService = new OfficeIPService();

            _rabbitMQ = new EasyNetQ(ConfigSettings.RabbitMQ_Host() + ":" + ConfigSettings.RabbitMQ_Port(),
                ConfigSettings.RabbitMQ_VirtualHost(),
                Convert.ToInt32(ConfigSettings.RabbitMQ_PrefetchCount()),
                ConfigSettings.RabbitMQ_Username(),
                ConfigSettings.RabbitMQ_Password());

            _rabbitMQ
                .CreateBus()
                .DeclareExchange(ConfigSettings.RabbitMQ_Exchange());

            ViewBag.IsHost = UserService.IsHavingPermission(CurrentSession.UserId, int.Parse(AppSettings.Get("SiteId")), FSFunctionKeys.FraudCase, Core.Business.Constant.Policy.Host);
        }

        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_Get)]
        public ActionResult Index()
        {            
            var fraudCaseViewModel = new FraudCaseViewModel();
            fraudCaseViewModel.FraudCaseInfo = Library.Models.FraudCase.FraudCase.GetFraudCase(FraudCaseId.BSNG);
            fraudCaseViewModel.FraudCaseList = _fraudCaseService.Get(null, null, envConfig);
            fraudCaseViewModel.FraudCaseInfo.IsActive = _fraudCaseService.CheckActiveFraudCase(FraudCaseId.BSNG, envConfig);

            return View(fraudCaseViewModel);
        }

        [HttpPost]       
        public ActionResult Index(int fraudCaseId)
        {
            var caseid = (FraudCaseId)fraudCaseId;
            var fraudCaseViewModel = new FraudCaseViewModel();
            fraudCaseViewModel.FraudCaseInfo = Library.Models.FraudCase.FraudCase.GetFraudCase(caseid);
            fraudCaseViewModel.FraudCaseList = _fraudCaseService.Get(null, null, int.Parse(ConfigurationManager.AppSettings["Environment"]));
            fraudCaseViewModel.FraudCaseInfo.IsActive = _fraudCaseService.CheckActiveFraudCase(caseid, envConfig);

            return View(fraudCaseViewModel);
        }

        [HttpPost]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_BSNG_UpdateStatus, "fraudCaseId"
            , "FCRule_BSNG_UpdateStatus=1"
            , "FCRule_OfficeIP_UpdateStatus=2"
            , "FCRule_CheckBet_UpdateStatus=3"
            , "FCRule_DailyData_UpdateStatus=4"
            , "FCRule_FlaggedAccount_UpdateStatus=6"
            , "FCRule_DelayGoal_UpdateStatus=8"
            , "FCRule_Arbitrage_UpdateStatus=10"
            , "FCRule_BetNearGoal_UpdateStatus=11"
            , "FCRule_GroupBetting_UpdateStatus=13"
            , "FCRule_Hedging_UpdateStatus=14")]
        public ActionResult ChangeCaseStatus(int fraudCaseId, bool isActive)
        {
            _fraudCaseService.ChangeFraudCaseStatus((FraudCaseId)fraudCaseId, CurrentSession.UserId);
            var fraudCaseName = Enum.GetName(typeof(FraudCaseId), fraudCaseId);
            if (!string.IsNullOrWhiteSpace(fraudCaseName))
            {
                _rabbitMQ
                    .DeclareQueue(fraudCaseName)
                    .DeclareRoutingKey(fraudCaseName)
                    .Bind();

                if (!string.IsNullOrWhiteSpace(_rabbitMQ.QueueName))
                {
                    var message = new Message<FraudCaseMessage>(new FraudCaseMessage
                    {
                        Action = isActive ? Model.Action.Activating : Model.Action.Deactivating,
                        Id = fraudCaseId,
                        Name = fraudCaseName,
                        Message = "Message is sent at" + DateTime.Now
                    }, new MessageProperties { DeliveryMode = 2, Priority = 9 });

                    _rabbitMQ.AdvanceBus.PublishAsync(_rabbitMQ.Exchange, _rabbitMQ.RoutingKey, false, message);
                }
            }

            return Json(new object());
        }

        #region delay goal customer
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_DelayGoal_GetCust)]
        public ActionResult GetDelayedGoalCustomerList()
        {
            var json = new
            {
                data = _fraudCaseService.GetDelayedGoalCustomers().Select((c, i) => new { rowNum = i + 1, data = c })
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.FraudCase, Policy = Core.Business.Constant.Policy.Host)]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_DelayGoal_DeleteCust)]
        public ActionResult DeleteDelayedGoalCustomer(int custId)
        {
            return Json(_fraudCaseService.DeleteDelayedGoalCustomer(custId, CurrentSession.UserId));
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.FraudCase, Policy = Core.Business.Constant.Policy.Host)]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_DelayGoal_AddCust)]
        public ActionResult AddDelayedGoalCustomer(string username)
        {
            var dbresult = _fraudCaseService.AddDelayedGoalCustomer(username, CurrentSession.UserId);

            if (dbresult.ErrorCode == 0)
            {
                var manualAccount = dbresult.OutParam["addedcustid"].ToString();
                /*IEnumerable<CustomerAssociation> customerAssociation =
                    ApiHelper.Post<string, IEnumerable<CustomerAssociation>>(
                        FPS_WEBAPI_GET_ASSOCIATION_ACCOUNT, manualAccount,
                        int.Parse(ConfigurationManager.AppSettings["Environment"]),
                        int.Parse(ConfigurationManager.AppSettings["MailingListId"])).Result;
                */
                var customerAssociation = ApiHelper.Post<IEnumerable<CustomerAssociation>>(FPS_WEBAPI_GET_ASSOCIATION_ACCOUNT, new { CustIdList = manualAccount }).Result;
                
                if (customerAssociation.Any())
                {
                    var customerAssociationXML = customerAssociation.ToXmlString();
                    dbresult = _delayGoalAccountService.InsertAccounts(new DelayGoalAssociationAccountInsertCriteria { CustomerAssociationXML = customerAssociationXML });
                }
            }

            return Json(dbresult);
        }

        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_DelayGoal_ExportToExcel)]
        public ActionResult ExportDelayedGoalCustomer(string searchString)
        {
            var templatePath = Server.MapPath("~/App_Data/Excel Template/FraudCase/DelayedGoal/template.xlsx");

            var fileName = string.Format("DelayGoalCustomerList_{0}.xlsx", DateTime.Today.ToString("yyyyMMdd"));

            return File(_fraudCaseService.ExportDelayedCustomersToExcel(templatePath, searchString), "application/vnd.ms-excel", fileName);
        }

        #endregion delay goal customer

        #region scanned office ip
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_IP_Get)]
        public ActionResult GetIpList()
        {
            return JData(_ipService.GetIps());
        }

        [NapAuth(FunctionKey = FSFunctionKeys.FraudCase, Policy = Core.Business.Constant.Policy.Host)]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_IP_Delete)]
        public ActionResult DeleteIpRange(string fromIp, string toIp)
        {
            return Json(_ipService.DeleteIpRange(fromIp, toIp, CurrentSession.UserId));
        }

        [NapAuth(FunctionKey = FSFunctionKeys.FraudCase, Policy = Core.Business.Constant.Policy.Host)]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_IP_Add)]
        public ActionResult AddIpRange(string fromIp, string toIp)
        {
            return Json(_ipService.AddIpRange(fromIp, toIp, CurrentSession.UserId));
        }

        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_IP_ExportToExcel)]
        public ActionResult ExportExcelIp()
        {
            var template = Server.MapPath("~/App_Data/Excel Template/FraudCase/ScannedOfficeIP/template.xlsx");

            var file = RenderExcelFile(
                    template,
                    wb => new OfficeIPExcelConfig().RenderExcel(wb, null, _ipService.GetIps(), null)
                );

            return ExcelFile(file, $"IPList_{DateTime.Today.ToString("yyyyMMdd")}");
        }

        #endregion scanned office ip

        #region scanned office internal account     
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_InternalAccount_Get)]
        public ActionResult GetAllInternalAccount()
        {
            return JData(_ipService.GetInternalAccounts());
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.FraudCase, Policy = Core.Business.Constant.Policy.Host)]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_InternalAccount_Add)]
        public ActionResult AddInternalAccount(string userName)
        {
            return Json(_ipService.AddInternalAccount(userName, CurrentSession.UserId));
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.FraudCase, Policy = Core.Business.Constant.Policy.Host)]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_InternalAccount_Delete)]
        public ActionResult DeleteInternalAccount(int custId)
        {
            return Json(_ipService.DeleteInternalAccount(custId, CurrentSession.UserId));
        }

        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_InternalAccount_ExportToExcel)]
        public ActionResult ExportExcelInternalAccount()
        {
            var template = Server.MapPath("~/App_Data/Excel Template/FraudCase/ScannedOfficeIP/InternalAccountTemplate.xlsx");

            var file = RenderExcelFile(
                    template,
                    wb => new OfficeIPInternalAccountExcelConfig().RenderExcel(wb, null, _ipService.GetInternalAccounts(), null)
                );

            return ExcelFile(file, $"InternalAccountList_{DateTime.Today.ToString("yyyyMMdd")}");
        }
        #endregion scanned office internal account

        #region notification recipient
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_Recipient_Get)]
        public ActionResult GetRecipients(FraudCaseId fraudCase)
        {
            return JData(_recipientService.GetRecipients(fraudCase));
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.FraudCase, Policy = Core.Business.Constant.Policy.Host)]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_Recipient_Add)]
        public ActionResult AddRecipients(string recipients, FraudCaseId fraudCase)
        {
            return Json(_recipientService.AddRecipient(recipients, CurrentSession.UserId, fraudCase));
        }

        [NapAuth(FunctionKey = FSFunctionKeys.FraudCase, Policy = Core.Business.Constant.Policy.Host)]
        [TrackingFuncUsageFilter(SystemId.FS, TrackingFunctionId.FCRule_Recipient_Delete)]
        public ActionResult RemoveRecipient(int id, FraudCaseId fraudCase)
        {
            return Json(_recipientService.RemoveRecipient(id, CurrentSession.UserId, fraudCase));
        }

        #endregion notification recipient
    }
}