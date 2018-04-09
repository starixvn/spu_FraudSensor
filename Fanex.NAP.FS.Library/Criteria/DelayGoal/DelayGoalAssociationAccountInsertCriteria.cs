namespace Fanex.NAP.FS.Library.Criteria
{
    using Data.Repository;

    public class DelayGoalAssociationAccountInsertCriteria : CriteriaBase
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
