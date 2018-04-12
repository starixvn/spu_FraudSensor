namespace Fanex.NAP.FS.Library.Criteria
{
    using Data.Repository;

    public class BigStakeNearGoalPreconditionCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "FS_FraudCase_BSNG_PreCondition";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}