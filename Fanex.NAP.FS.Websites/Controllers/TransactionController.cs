namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.FS.Library.Services;
    using Fanex.NAP.FS.Library.ViewModels.Transaction;
    using System.Web.Mvc;
    using Fanex.NAP.Core.Web.Attribute;

    [NapAuth(FunctionKey = FSFunctionKeys.TransactionHistory, Policy = Core.Business.Constant.Policy.Read)]
    public class TransactionController : Controller
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
        public ActionResult GetTransactionHistory(int accountId)
        {
            return PartialView("TransactionHistory");
        }

        [HttpPost]
        public ActionResult GetTransactionHistoryFilter(TransactionHistoryFilter filter)
        {
            var data = _transactionService.GetTransactionHistory(filter);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}