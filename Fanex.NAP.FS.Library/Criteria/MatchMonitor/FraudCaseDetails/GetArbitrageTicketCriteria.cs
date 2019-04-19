namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails
{
    public class GetArbitrageTicketCriteria : ArbitrageDetailsCriteria
    {      
        public override string GetSettingKey() => "FS_Rpt_MatchMonitor_Details_Get_ByTicket_Arbitrage";

        public override bool IsValid() => true;
    }
}