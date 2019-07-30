namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails
{
    using Data;
    using Fanex.Data.Repository;
    using System;

    [Serializable]
    public class ArbitrageDetailsCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        public DateTime EventDate { get; set; }

        public bool IsShowSpecialCurrencies { get; set; }

        public int LiveStatus { get; set; }

        public string CurrencyIds { get; set; }

        [SpParam(Ignored = true)]
        public string CurrencyNames { get; set; }

        [SpParam(Ignored = true)]
        public string EvidenceCodeList { get; set; }

        [SpParam(Ignored = true)]
        public string TransDateFromString { get; set; }

        public DateTime TransDateFrom
        {
            get
            {
                DateTime minValue;

                if (DateTime.TryParse(TransDateFromString, out minValue))
                {
                    return minValue;
                }

                return new DateTime(1753, 1, 1);
            }
        }

        [SpParam(Ignored = true)]
        public string TransDateToString { get; set; }

        public DateTime TransDateTo
        {
            get
            {
                DateTime minValue;

                if (DateTime.TryParse(TransDateToString, out minValue))
                {
                    return minValue;
                }

                return new DateTime(9999, 12, 31);
            }
        }

        [SpParam(Ignored = true)]
        public string KeyName { get; set; }

        public int Bettype { get; set; }

        public float Handicap { get; set; }

        public int LiveHomeScore { get; set; }

        public int LiveAwayScore { get; set; }

        [SpParam(Ignored = true)]
        public int GroupId { get; set; }

        [SpParam(Ignored = true)]
        public string Sites { get; set; }

        [SpParam(Ignored = true)]
        public string Score { get; set; }

        [SpParam(Ignored = true)]
        public int TotalTicket { get; set; }

        [SpParam(Ignored = true)]
        public bool IsOnlyCN88 { get; set; }

        [SpParam(Ignored = true)]
        public bool IsByIPAndDevice { get; set; }

        public string Site { get; set; }

        public double? Odds { get; set; }

        public override string GetSettingKey()
        {
            return string.Empty;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}