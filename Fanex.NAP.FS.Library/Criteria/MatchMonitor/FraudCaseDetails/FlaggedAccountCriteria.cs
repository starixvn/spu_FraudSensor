namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails
{
    using Data;
    using Fanex.Data.Repository;
    using System;

    [Serializable]
    public class FlaggedAccountCriteria : CriteriaBase
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

        public int Bettype { get; set; }
    
        [SpParam(Ignored = true)]
        public string Sites { get; set; }

        [SpParam(Ignored = true)]
        public bool SaveChoice { get; set; }  

        public override string GetSettingKey() => "";

        public override bool IsValid() => true;

    }
}
