namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor.FraudCaseDetails
{    
    using Fanex.Data.Repository;
    public class GetCurrencyCriteria : CriteriaBase
    {
        public bool IsShowSpecialCurrencies {get; set;}

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
