namespace Fanex.NAP.FS.WinService.Console
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Core.Helpers.Api;
    using Fanex.NAP.FS.Library.Core.Utilities;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.FraudCaseTask;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var lines = File.ReadLines(ConfigurationManager.AppSettings["export"]);

            var ids = lines.Skip(1).Select(int.Parse).ToArray();

            for (int i = 0; i < 100; i++)
            {
                var tsks = ids.Chunk(500).Select(a => ApiHelper.Post<IEnumerable<ArbitrageCustomer>>(AppSettings.MS_WEBAPI_GROUP_CUSTOMER, a)).ToArray();

                Task.WaitAll(tsks);
            }
        }
    }
}