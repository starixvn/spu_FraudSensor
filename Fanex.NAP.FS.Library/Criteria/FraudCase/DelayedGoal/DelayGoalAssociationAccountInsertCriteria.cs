namespace Fanex.NAP.FS.Library.Criteria.FraudCase.DelayedGoal
{
    using Data.Repository;
    using Fanex.NAP.FS.Library.Core.BaseClasses;

    public class DelayGoalAssociationAccountInsertCriteria : CommandBase
    {
        public string CustomerAssociationXML { get; set; }

        public override string GetSettingKey()
        {
            return "FS_DelayGoalAccount_ImportFromFPS";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}