namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.FS.Library.Services;
    using System.Web.Mvc;

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

        public ActionResult GetTransactionHistory(int accountId)
        {
            var data = _transactionService.GetTransactionHistory(accountId);

            return PartialView("TransactionHistory", data);
        }
    }
}