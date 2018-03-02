using System.Web.Optimization;

namespace Fanex.NAP.FS.Websites
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-1.12.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/select2.min.css",
                      "~/Content/style.css",
                      "~/Content/index.min.css",
                      "~/Content/themes/base/jquery-ui.min.css"));

            RegisterFSCoreBundles(bundles);
            RegisterMatchMonitorBundles(bundles);
        }

        private static void RegisterFSCoreBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/core").Include(
                "~/Scripts/select2.min.js",
                "~/Scripts/common.js",
                "~/Scripts/Fanex.Grid/Fanex.Grid.js",
                "~/Scripts/Fanex.Grid/Fanex.Pagination.js",
                "~/Scripts/auto-grid.js"));
        }

        private static void RegisterMatchMonitorBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/match-monitor").Include("~/scripts/MatchMonitor/*.js"));
        }
    }
}