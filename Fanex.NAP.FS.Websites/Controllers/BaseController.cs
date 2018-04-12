namespace Fanex.NAP.FS.Websites.Controllers
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Context;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;

    public class BaseController : Controller
    {
        private readonly string _environment = NapSettings.Settings == null ? string.Empty : NapSettings.Settings.Environment;
        private readonly int _siteId = NapSettings.Settings == null ? 0 : NapSettings.Settings.SiteId;
        private readonly bool _isSiteLogin = (NapSettings.Settings != null) && NapSettings.Settings.EnableDirectLogin;

        public BaseController()
        {
            CurrentContext = new CurrentContext();
            UserService = new UserService();

            if (NapSettings.Settings != null)
            {
                CurrentSession = new CurrentSession();
                ViewBag.Session = CurrentSession;
            }
        }

        public int SiteId
        {
            get { return _siteId; }
        }

        public bool IsSiteLogin
        {
            get { return _isSiteLogin; }
        }

        public string Environment
        {
            get { return _environment; }
        }

        public ICurrentContext CurrentContext
        {
            get;
            set;
        }

        public ICurrentSession CurrentSession
        {
            get;
            set;
        }

        public IUserService UserService { get; }

        /// <summary>
        /// Return the HTML string of a View.
        /// </summary>
        /// <param name="context">Controller context.</param>
        /// <param name="viewPath">View path.</param>
        /// <param name="viewModel">View model.</param>
        /// <param name="partial">View partial.</param>
        /// <returns>HTML string.</returns>
        public string RenderViewToString(
            ControllerContext context,
            string viewPath,
            object viewModel = null,
            bool partial = false)
        {
            ViewEngineResult viewEngineResult = null;
            viewEngineResult = partial ? ViewEngines.Engines.FindPartialView(context, viewPath) : ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null)
            {
                throw new FileNotFoundException("View cannot be found.");
            }

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = viewModel;

            var htmlStringFromView = string.Empty;
            using (var stringWriter = new StringWriter())
            {
                var viewContext = new ViewContext(
                    context,
                    view,
                    context.Controller.ViewData,
                    context.Controller.TempData,
                    stringWriter);
                view.Render(viewContext, stringWriter);
                htmlStringFromView = stringWriter.ToString();
            }

            return htmlStringFromView;
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = int.MaxValue
            };
        }

        protected ActionResult ExcelFile(byte[] contents, string fileName, string extension = "xlsx")
        {
            return File(contents, "application/vnd.ms-excel", fileName + "." + extension);
        }

        protected new ActionResult Json(object data)
        {
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        protected ActionResult JData<T>(IEnumerable<T> data)
        {
            return Json(new { data });
        }
    }
}