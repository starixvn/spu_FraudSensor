namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails
{
    public class GetArbitrageCustomerCriteria : ArbitrageDetailsCriteria
    {
        public override string GetSettingKey()
        {
            return "FS_Rpt_MatchMonitor_Details_Get_ByCustomer_Arbitrage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
