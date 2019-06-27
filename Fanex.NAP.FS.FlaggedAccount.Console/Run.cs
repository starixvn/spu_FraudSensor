namespace Fanex.NAP.FS.FlaggedAccount.Console
{
    using Fanex.Data;
    using Fanex.Data.ConnectionStringProvider20;
    using Fanex.NAP.FS.Core;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.FraudCase.FlaggedAccount;
    using Fanex.NAP.FS.Service;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class Run
    {
        public void Start()
        {
            try
            {
                var _service = new Service();

                if (_service.CheckUM())
                    return;

                _service
                    .CleanResult()
                    .Precondition()
                    .Rule();
            }
            catch (Exception ex)
            {
                //var logMessage = new LogMessage(ex.Message)
                //    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                //    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                //LogService.Error($"{ProjectName} - {ServiceName} - Execute Exeption {logMessage}");
            }
        }
    }
}
