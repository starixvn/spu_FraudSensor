namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Criteria.FraudCase.ScannedOfficeIP;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Models.FraudCase.ScannedOfficeIP;
    using Fanex.NAP.FS.Library.Services.Interface;
    using System.Collections.Generic;

    public class OfficeIPService : IOfficeIPService
    {
        public DBResult AddIpRange(string fromIp, string toIp, int userId)
        {
            var from_arr = fromIp.Split('.');
            var to_arr = toIp.Split('.');

            var criteria = new OfficeIPInsertCriteria
            {
                IPRange = $"{from_arr[0]}.{from_arr[1]}.{from_arr[2]}",
                FromNumber = int.Parse(from_arr[3]),
                ToNumber = int.Parse(to_arr[3]),
                UserId = userId
            };

            return DataService.UpdateData(criteria, "OfficeIP");
        }

        public DBResult DeleteIpRange(string fromIp, string toIp, int userId)
        {
            var from_arr = fromIp.Split('.');
            var to_arr = toIp.Split('.');

            var criteria = new OfficeIPDeleteCriteria
            {
                IPRange = $"{from_arr[0]}.{from_arr[1]}.{from_arr[2]}",
                FromNumber = int.Parse(from_arr[3]),
                ToNumber = int.Parse(to_arr[3]),
                UserId = userId
            };

            return DataService.UpdateData(criteria, "OfficeIP");
        }        

        public IEnumerable<OfficeIp> GetIps()
        {
            return DataService.GetData<OfficeIp>(new OfficeIPGetCriteria());
        }

        #region Exclude Internal Account
        public IEnumerable<OfficeIPInternalAccount> GetInternalAccounts()
        {
            return DataService.GetData<OfficeIPInternalAccount>(new OfficeIPInternalAccountGetCriteria());
        }

        public DBResult AddInternalAccount(string userName, int userId)
        {
            var criteria = new OfficeIPInternalAccountInsertCriteria
            {
                UserId = userId,
                Username = userName
            };

            return DataService.UpdateData(criteria, "FraudCase_OfficeIPInternalAccount");
        }

        public DBResult DeleteInternalAccount(int custId, int userId)
        {
            var criteria = new OfficeIPInternalAccountDeleteCriteria
            {
                CustId = custId,
                UserId = userId,
                IsActive = false
            };

            return DataService.UpdateData(criteria, "FraudCase_OfficeIPInternalAccount");
        }
        #endregion Exclude Internal Account
    }
}