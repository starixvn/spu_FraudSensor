namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models.SuspiciousCustomerDetails;
    using System;
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.Criteria.SuspiciousCustomerDetails;

    public class SuspiciousCustomerReportDetailsService : ISuspiciousCustomerReportDetailsService
    {
        #region statistics

        public IEnumerable<Summary> GetSummaries(int custid, DateTime from, DateTime to)
        {
            var criteria = new SuspiciousCustomerDetailsSummaryGetCriteria();

            return new[] { new Summary { Turnover = 13_801_300.00, WinLoss = -602_555.50, BetCount = 546 } };
        }

        public IEnumerable<ChoiceStatistics> GetChoices(int custid, DateTime from, DateTime to)
        {
            var criteria = new SuspiciousCustomerDetailsChoiceGetCriteria();

            return new[]
            {
                    new ChoiceStatistics { Choice = "Live", Turnover = 12116500.00, WinLoss = -645659.00 },
                    new ChoiceStatistics { Choice = "None-Live", Turnover = 1684800.00, WinLoss = 43103.50 }
            };
        }

        public IEnumerable<SportStatistics> GetTop5Sports(int custid, DateTime from, DateTime to)
        {
            var criteria = new SuspiciousCustomerDetailsTop5SportsGetCriteria();

            return new[]
                {
                    new SportStatistics { SportName = "Soccer", Turnover = 12116500.00, WinLoss = -645659.00 }
                };
        }

        public IEnumerable<BettypeStatistics> GetTop5Bettypes(int custid, DateTime from, DateTime to)
        {
            var criteria = new SuspiciousCustomerDetailsTop5BettypesGetCriteria();

            return new[]
                    {
                        new BettypeStatistics { BetTypeName = "Handicap", Turnover = 12116500.00, WinLoss = -645659.00 },
                        new BettypeStatistics { BetTypeName = "Over/Under", Turnover = 12116500.00, WinLoss = -645659.00 }
                    };
        }

        #endregion statistics
    }
}