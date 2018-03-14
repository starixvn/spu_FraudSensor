using System.Web.Optimization;

namespace Fanex.NAP.FS.Websites
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;

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

            bundles.Add(new StyleBundle("~/Content/styles").Include(
                      "~/Content/scss/vendors/select2.min.css",
                      "~/Content/scss/vendors/jquery-ui/base/jquery-ui.min.css",
                      "~/Content/scss/vendors/DataTables/css/jquery.dataTables.min.css",
                      "~/Content/scss/fonts/Fraud-Sensor/style.css",
                      "~/Content/scss/base/reset.css",
                      "~/Content/scss/pages/index.css"));

            RegisterFSCoreBundles(bundles);
            RegisterMatchMonitorBundles(bundles);
            RegisterFraudCaseBundles(bundles);
        }

        private static void RegisterFSCoreBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/core").Include(
                "~/Scripts/select2.min.js",
                "~/Scripts/common.js",
                "~/Scripts/Fanex.Grid/Fanex.Grid.js",
                "~/Scripts/Fanex.Grid/Fanex.Pagination.js",
                "~/Scripts/DataTables/jquery.datatables.min.js"));
        }

        private static void RegisterMatchMonitorBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/match-monitor").Include("~/scripts/MatchMonitor/*.js"));
        }

        private static void RegisterFraudCaseBundles(BundleCollection bundles)
        {
           bundles.Add(new StyleBundle("~/fraud-case-styles").Include("~/content/scss/pages/configurations/fraud-case.css"));

            bundles.Add(new ScriptBundle("~/fraud-case-scripts").Include("~/scripts/fraudcase/fraud-case.js"));

            bundles.Add(new ScriptBundle("~/fraud-case-scripts/delayed-goal").Include("~/scripts/fraudcase/delayed-goal.js"));
        }
    }
}