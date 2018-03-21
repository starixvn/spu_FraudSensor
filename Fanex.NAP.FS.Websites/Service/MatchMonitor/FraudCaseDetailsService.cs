using Fanex.NAP.FS.Websites.ViewModels.MatchMonitor.FraudCaseDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NAP.FS.Websites.Service.MatchMonitor
{
    public static class FraudCaseDetailsService
    {
        public static IEnumerable<TIcketDetails> GetTicketDetails(int matchId, int fraudCaseId)
        {
            for (int i = 0; i < 100; i++)
            {
                yield return new TIcketDetails
                {
                    Member = "Simon",
                    TransDate = DateTime.Now.ToString("MMM dd, yyyy hh:mm:ss tt"),
                    BetID = Guid.NewGuid().ToString(),
                    Choice = "Topology",
                    Score = "0-0",
                    Odds = "afadf",
                    Stake = 59873.554,
                    WinLoss = 458951.58,
                    Status = "Won",
                    IP = "192.168.1.1",
                    Bettype = (i / 10).ToString()
                };
            }
        }

        public static IEnumerable<CustomerDetails> GetCustomerDetails(int matchId, int fraudCaseId)
        {
            for (int i = 0; i < 100; i++)
            {
                yield return new CustomerDetails
                {
                    Member = "Simon",
                    Tickets = 500,
                    TotalStake = 1000,
                    TotalWinLoss = 86247.578489,
                    Evidence = "dafadfdaf,adfdfadf",
                    AssociatedAccounts = 15
                };
            }
        }
    }
}