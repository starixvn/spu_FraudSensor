namespace Fanex.NAP.FS.Library.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class RecipientGetCriteria : CriteriaBase
    {
        public int FraudCaseId { get; set; }

        public override string GetSettingKey() => "FS_Recipients_Get";

        public override bool IsValid() => true;
    }
}