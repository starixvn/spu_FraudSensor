namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails
{    
    using Fanex.Data.Repository;
    public class GetCurrencyCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "FS_Rpt_Currency_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
