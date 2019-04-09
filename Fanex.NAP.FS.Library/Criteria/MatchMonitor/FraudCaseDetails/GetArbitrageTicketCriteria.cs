namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails
{
    using Data;
    using Fanex.Data.Repository;
    using System;

    public class GetArbitrageTicketCriteria : CriteriaBase
    {
        private DateTime _transDateFrom;
        private DateTime _transDateTo;

        public int MatchId { get; set; }

        public DateTime EventDate { get; set; }

        public bool IsShowSpecialCurrencies { get; set; }

        public int LiveStatus { get; set; }

        public int CurrencyId { get; set; }

        [SpParam(Ignored = true)]
        public string EvidenceCodeList { get; set; }

        public DateTime TransDateFrom
        {
            get
            {
                DateTime minValue = new DateTime(1753, 1, 1);
                DateTime maxValue = new DateTime(9999, 1, 1);
                if (_transDateTo > minValue && _transDateTo < maxValue)
                {
                    return _transDateFrom;
                }
                return minValue;
            }
            set
            {
                _transDateFrom = value;
            }
        }

        public DateTime TransDateTo
        {
            get
            {
                DateTime minValue = new DateTime(1753, 1, 1);
                DateTime maxValue = new DateTime(9999, 1, 1);
                if (_transDateTo > minValue && _transDateTo < maxValue)
                {
                    return _transDateTo;
                }
                return maxValue;
            }
            set
            {
                _transDateTo = value;
            }
        }

        public int Bettype { get; set; }

        public float Handicap { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        [SpParam(Ignored = true)]
        public int TotalTicket { get; set; }

        public override string GetSettingKey() => "FS_Rpt_MatchMonitor_Details_Get_ByTicket_Arbitrage";

        public override bool IsValid() => true;
    }
}