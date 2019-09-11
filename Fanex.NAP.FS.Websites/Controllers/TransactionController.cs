namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.FS.Library.Services;
    using Fanex.NAP.FS.Library.ViewModels.Transaction;
    using System.Web.Mvc;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.FS.Library.Models.Transaction;

    public class TransactionController : BaseController
    {
        private readonly ITransactionService _transactionService;

        public TransactionController()
        {
            _transactionService = new TransactionService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.TransactionHistory, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult GetTransactionHistory(int accountId)
        {
            return PartialView("TransactionHistory");
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.TransactionHistory, Policy = Core.Business.Constant.Policy.Read)]
        public ActionResult GetTransactionHistoryFilter(TransactionHistoryFilter filter)
        {
            var data_test = new System.Collections.Generic.List<Transaction>();
            var data = _transactionService.GetTransactionHistory(filter);
            
            for (int i = 0; i < 50; i++)
            {
                data_test.AddRange(data);
            }            

            return Json(data_test, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetIPInformation(string ip)
        {
            var data = _transactionService.GetIPInformation(ip);

            return PartialView("IPInformation", data);
        }
    }
}