namespace Fanex.NAP.FS.Library.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class RecipientRemoveCriteria : CriteriaBase
    {
        public int RecipientId { get; set; }

        public int UserId { get; set; }
        public int FraudCaseId { get; internal set; }

        public override string GetSettingKey() => "FS_Recipients_Delete";

        public override bool IsValid() => true;
    }
}