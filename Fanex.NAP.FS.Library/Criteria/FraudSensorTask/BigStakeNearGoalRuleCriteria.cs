namespace Fanex.NAP.FS.Library.Criteria
{
    using Data.Repository;

    public class BigStakeNearGoalRuleCriteria : CriteriaBase
    {
        public string MatchIds { get; set; }

        public override string GetSettingKey()
        {
            return "FS_FraudCase_BSNG_Rule";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
