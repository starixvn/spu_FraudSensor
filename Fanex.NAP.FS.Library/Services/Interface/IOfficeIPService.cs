namespace Fanex.NAP.FS.Library.Services.Interface
{
    using Fanex.NAP.FS.Library.Criteria.ScannedOfficeIP;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.FraudCase.ScannedOfficeIP;
    using System.Collections.Generic;

    public interface IOfficeIPService
    {
        IEnumerable<OfficeIp> GetIps();

        DBResult AddIpRange(string fromIp, string toIp, int userId);

        DBResult DeleteIpRange(string fromIp, string toIp, int userId);

        IEnumerable<OfficeIPInternalAccount> GetInternalAccounts();

        DBResult AddInternalAccount(string userName, int userId);

        DBResult DeleteInternalAccount(int custId, int userId);

        DBResult InsertOfficeIpTrans(IEnumerable<OfficeIpTransaction> transactions);
    }
}