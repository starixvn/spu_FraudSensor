namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails
{
    using Fanex.Data.Repository;
    using System;

    public class GetArbitrageTicketCriteria:CriteriaBase
    {
        public int MatchId { get; set; }

        public DateTime EventDate { get; set; }

        public override string GetSettingKey() => "FS_Rpt_MatchMonitor_Details_Get_ByTicket_Arbitrage";

        public override bool IsValid() => true;
    }
}