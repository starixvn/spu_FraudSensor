namespace Fanex.NAP.FS.Library.Services
{
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.Models.Transaction;
    using Fanex.NAP.FS.Library.ViewModels.Transaction;
    using Fanex.NAP.FS.Library.Criteria.Transaction.History;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Criteria.Transaction;
    using System.Linq;
    using Fanex.NAP.FS.Library.Models.IP;
    using System.Net;
    using System;
    using System.Numerics;

    public class TransactionService : ITransactionService
    {
        public IEnumerable<Transaction> GetTransactionHistory(TransactionHistoryFilter filter)
        {
            var criteria = new TransactionHistoryGetByCustIdCriteria
            {
                ip_accountId = filter.AccountId,
                ip_historyRange = (int)filter.ReportLength
            };

            return DataService.GetData<Transaction>(criteria);
        }

        public IPInformation GetIPInformation(string ipAddress)
        {
            var ipModel = Ip2Long(ipAddress);

            var criteria = new IPInformationGetByRangeLocationCriteria
            {
                ip_IP = decimal.Parse(ipModel.IpNumber.ToString())
            };

            var data = DataService.GetData<IPInformation>(criteria);

            if (data.Any())
            {
                return data.ToList()[0];
            }

            return new IPInformation();
        }

        private IpModel Ip2Long(string ipAddress)
        {
            var ipAd = IPAddress.Parse(ipAddress);

            BigInteger ipnum = 0;

            byte[] addrBytes = ipAd.GetAddressBytes();

            if (BitConverter.IsLittleEndian)
            {
                var byteList = new List<byte>(addrBytes);
                byteList.Reverse();
                addrBytes = byteList.ToArray();
            }

            if (addrBytes.Length > 8)
            {
                //IPv6
                ipnum = BitConverter.ToUInt64(addrBytes, 8);
                ipnum <<= 64;
                ipnum += BitConverter.ToUInt64(addrBytes, 0);

                return new IpModel
                {
                    IpNumber = ipnum,
                    IsIpV6 = true
                };
            }
            else
            {
                //IPv4
                ipnum = BitConverter.ToUInt32(addrBytes, 0);

                return new IpModel
                {
                    IpNumber = ipnum,
                    IsIpV6 = false
                };
            }
        }
    }
}