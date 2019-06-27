using Fanex.Data;
using Fanex.Data.ConnectionStringProvider20;
using Fanex.NAP.FS.Core;
using Fanex.NAP.FS.Library.Models;
using Fanex.NAP.FS.Model;
using ONEbook.UM.Services;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Service
{
    public class BaseService
    {
        public int FraudCaseId { get; set; }

        protected MaintenanceService maintenanceService;

        public BaseService(FraudCaseId fraudCaseId)
        {
            DbSettingProviderManager
                  .StartNewSession()
                  .Use(new ConnectionStringProvider20())
                  .UseDefaultDbSettingProvider(AppSetting.AppData, false)
                  .Run();

            //LogManager.SetDefaultLogCategory(AppSetting.FanexLogCategory)
            //         .Use(new List<ILogEngine>
            //         {
            //             new SentryLogging(new SentryEngineOptions
            //             {
            //                 Dsn = new Dsn(AppSetting.SentryLogging)
            //             })
            //         });

            FraudCaseId = Convert.ToInt32(fraudCaseId);

            maintenanceService = new MaintenanceService();
        }

        public virtual BaseService Precondition() { return this; }

        public virtual BaseService Rule() { return this; }

        public virtual BaseService CleanResult() { return this; }

        public void Report(ReportModel model) {

            var client = new RestClient(AppSetting.HeartBeatUrl);
            var request = new RestRequest("/api/HealthCheck/beat", Method.POST);

            request.AddQueryParameter("fraudcaseid", this.FraudCaseId.ToString());
            request.AddQueryParameter("status", model.Status.ToString());
            request.AddQueryParameter("exception", model.Exception);
            request.AddQueryParameter("timeout", model.Timeout.ToString());
            request.AddQueryParameter("isActive", model.IsActive.ToString());
            request.AddQueryParameter("message", model.Message);

            client.Execute(request);
        }

        public bool CheckUM()
        {
            return maintenanceService.IsUnderMaintenance().Result;
        }
    }
}
