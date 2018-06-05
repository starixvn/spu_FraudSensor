namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.FraudCase.ScannedOfficeIP;
    using Fanex.NAP.FS.Library.Services.Interface;
    using System.Collections.Generic;

    public class OfficeIPService : IOfficeIPService
    {
        public DBResult AddIpRange(string fromIp, string toIp)
        {
            return new DBResult
            {
                ErrorMsg = $"{fromIp} - {toIp}"
            };
        }

        public DBResult DeleteIpRange(string fromIp, string toIp)
        {
            return new DBResult
            {
                ErrorMsg = $"{fromIp} - {toIp}"
            };
        }

        public IEnumerable<OfficeIp> GetIps()
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new OfficeIp
                {
                    FromIp = "192.158.2.1",
                    ToIp = "192.5.1.7"
                };
            }
        }
    }
}