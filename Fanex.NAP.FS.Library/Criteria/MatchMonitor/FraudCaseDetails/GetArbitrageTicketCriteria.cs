namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails
{
    using Data;
    using Fanex.Data.Repository;
    using System;

    public class GetArbitrageTicketCriteria : CriteriaBase
    {
       
        public int MatchId { get; set; }

        public DateTime EventDate { get; set; }

        public bool IsShowSpecialCurrencies { get; set; }

        public int LiveStatus { get; set; }

        public int CurrencyId { get; set; }

        public DateTime TransDateFrom { get; set; }

        public DateTime TransDateTo { get; set; }

        public int Bettype { get; set; }

        public float Handicap { get; set; }

        public int TotalScores { get; set; }
     
        public override string GetSettingKey() => "FS_Rpt_MatchMonitor_Details_Get_ByTicket_Arbitrage";

        public override bool IsValid() => true;
    }
}