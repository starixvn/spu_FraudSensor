namespace Fanex.NAP.FS.Websites
{
    using System.Web.Optimization;

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

            bundles.Add(new StyleBundle("~/Content/styles").Include(
                      "~/Content/scss/vendors/select2.min.css",
                      "~/Content/scss/vendors/jquery-ui/base/jquery-ui.min.css",
                      "~/Content/scss/vendors/DataTables/css/jquery.dataTables.min.css",
                      "~/Content/scss/vendors/toast/jquery.toast.css",
                      "~/Content/scss/vendors/multiple-select.css",
                      "~/Content/scss/base/reset.css",
                      "~/Content/scss/pages/index.css"));

            RegisterFSCoreBundles(bundles);
            RegisterMatchMonitorBundles(bundles);
            RegisterFraudCaseBundles(bundles);
            RegisterSuspiciousCustomerBundles(bundles);
            RegisterSuspiciousCustomerDetailsBundles(bundles);
            RegisterScannedOfficeIpBundles(bundles);
            RegisterNotificationBundles(bundles);
            RegisterDailyDataCaptureBundles(bundles);
            RegisterCheckBetReportBundles(bundles);
            RegisterCustomerMonitorBundles(bundles);
            RegisterSubscriberAssignment(bundles);
            RegisterSearchBundles(bundles);
            RegisterAssociatedAccountBundles(bundles);
            RegisterEvidenceBundles(bundles);
            RegisterTransactionHistoryBundles(bundles);
            RegisterGraphVisualizeBundles(bundles);
        }

        private static void RegisterFSCoreBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/core").Include(
                "~/Scripts/moment.min.js",
                "~/Scripts/select2.min.js",
                "~/Scripts/common.js",
                "~/Scripts/Fanex.Grid/Fanex.Grid.js",
                "~/Scripts/Fanex.Grid/Fanex.Pagination.js",
                "~/Scripts/DataTables/jquery.datatables.js",
                "~/Scripts/toast/jquery.toast.js",
                "~/Scripts/numeral/numeral.min.js",
                "~/scripts/multiple-select.js",
                "~/Scripts/search.js",
                "~/Scripts/toogle.js"));

            bundles.Add(new ScriptBundle("~/finalize").Include("~/scripts/finalize.js"));
        }

        private static void RegisterMatchMonitorBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/match-monitor").Include("~/scripts/MatchMonitor/*.js"));

            bundles.Add(new ScriptBundle("~/scripts/match-monitor/fraud-case-details").Include("~/scripts/matchmonitor/fraudcasedetails/*.js"));

            bundles.Add(new StyleBundle("~/Contents/CoreBetList").Include("~/Content/CoreBetList.css"));
        }

        private static void RegisterFraudCaseBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/fraud-case-styles").Include("~/content/scss/pages/configurations/fraud-case.css"));

            bundles.Add(new ScriptBundle("~/fraud-case-scripts").Include("~/scripts/fraudcase/fraud-case.js"));

            bundles.Add(new ScriptBundle("~/fraud-case-scripts/delayed-goal").Include("~/scripts/fraudcase/delayed-goal.js"));

            bundles.Add(new ScriptBundle("~/fraud-case-scripts/scanned-office-ip").Include("~/scripts/fraudcase/scanned-office-ip.js"));

            bundles.Add(new ScriptBundle("~/fraud-case-scripts/scanned-office-account").Include("~/scripts/fraudcase/scanned-office-account.js")); 

            bundles.Add(new ScriptBundle("~/fraud-case-scripts/daily-data-capture").Include("~/scripts/fraudcase/daily-data-capture.js"));
        }

        private static void RegisterSuspiciousCustomerBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/suspicious-customer-styles").Include("~/content/scss/pages/suspicious-customer/suspicious-customer.css"));

            bundles.Add(new StyleBundle("~/scripts/suspicious-customer").Include("~/scripts/suspiciouscustomer/suspicious-customer.js"));
        }

        private static void RegisterSuspiciousCustomerDetailsBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/suspicious-customer-scripts").Include(
                "~/scripts/suspiciouscustomerdetails/*.js"));
        }

        private static void RegisterScannedOfficeIpBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/ScannedOfficeIp-scripts/main").Include("~/scripts/ScannedOfficeIp/scanned-office-ip.js"));
        }
        
        private static void RegisterDailyDataCaptureBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/daily-data-capture-scripts").Include("~/scripts/dailydatacapture/daily-data-capture.js"));
        }

        private static void RegisterNotificationBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/notification").Include(
                "~/scripts/Notification/*.js"));
        }

        private static void RegisterCheckBetReportBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/check-bet-report-scripts").Include("~/scripts/CheckBetReport/check-bet-report.js"));
        }

        private static void RegisterCustomerMonitorBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/customer-monitor-styles").Include("~/content/scss/pages/customer-monitor/customer-monitor.css"));

            bundles.Add(new ScriptBundle("~/scripts/customer-monitor").Include("~/scripts/customermonitor/*.js"));

            bundles.Add(new ScriptBundle("~/scripts/customer-monitor/customer-details").Include("~/scripts/customermonitor/customerdetails/*.js"));
        }

        private static void RegisterSubscriberAssignment(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/subscriber-assignment").Include("~/scripts/SubscriberAssignment/*.js"));
        }

        private static void RegisterSearchBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/search-account-scripts").Include("~/scripts/Search/Account/*.js"));
        }

        private static void RegisterAssociatedAccountBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/account/associated-scripts").Include("~/scripts/Account/Associated/*.js"));
            bundles.Add(new ScriptBundle("~/account-scripts").Include("~/scripts/Account/*.js"));
        }

        private static void RegisterGraphVisualizeBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/graph-visualize-lib").Include(
                "~/Scripts/popoto/d3.min.js",
                "~/Scripts/popoto/popoto.min.js"));
        }

        private static void RegisterEvidenceBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/evidence-scripts").Include("~/scripts/Evidence/*.js"));           
        }

        private static void RegisterTransactionHistoryBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/transaction/history-scripts").Include("~/scripts/Transaction/History/*.js"));
        }

    }
}