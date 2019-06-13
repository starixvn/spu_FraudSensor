namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails
{
    using Data;
    using Fanex.Data.Repository;
    using System;

    [Serializable]
    public class GetFlaggedAccountCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        public DateTime EventDate { get; set; }

        public bool IsShowSpecialCurrencies { get; set; }

        [SpParam(Ignored = true)]
        public int LiveStatus { get; set; }

        [SpParam(Ignored = true)]
        public string CurrencyIds { get; set; }

        [SpParam(Ignored = true)]
        public string CurrencyNames { get; set; }

        [SpParam(Ignored = true)]
        public string EvidenceCodeList { get; set; }

        [SpParam(Ignored = true)]
        public string TransDateFromString { get; set; }

        [SpParam(Ignored = true)]
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

        [SpParam(Ignored = true)]
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
        public int Bettype { get; set; }        

        [SpParam(Ignored = true)]
        public string Sites { get; set; }

        [SpParam(Ignored = true)]
        public bool SaveChoice { get; set; }

        public int FraudCaseId { get; set; }

        public override string GetSettingKey() => "FS_Rpt_MatchMonitor_Details_Get_ByTicket";

        public override bool IsValid() => true;

    }
}
