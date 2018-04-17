namespace Fanex.NAP.FS.Library.Criteria.FraudSensorTask
{
    using Data.Repository;

    public class FlaggedAccountRuleCriteria : CriteriaBase
    {
        public string MatchIds { get; set; }

        public string FlaggedAccountsXML { get; set; }

        public override string GetSettingKey()
        {
            return "FS_FraudCase_FlaggedAccount_Rule";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}