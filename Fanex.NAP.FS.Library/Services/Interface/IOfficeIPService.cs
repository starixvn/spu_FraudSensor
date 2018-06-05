namespace Fanex.NAP.FS.Library.Services.Interface
{
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.FraudCase.ScannedOfficeIP;
    using System.Collections.Generic;

    public interface IOfficeIPService
    {
        IEnumerable<OfficeIp> GetIps();

        DBResult AddIpRange(string fromIp, string toIp);

        DBResult DeleteIpRange(string fromIp, string toIp);
    }
}