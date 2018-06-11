namespace Fanex.NAP.FS.Library.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class RecipientAddCriteria : CriteriaBase
    {
        public string RecipientIds { get; set; }

        public int UserId { get; set; }
        public int FraudCaseId { get; internal set; }

        public override string GetSettingKey() => "FS_Recipients_Add";

        public override bool IsValid() => true;
    }
}