namespace Fanex.NAP.FS.Library.Criteria.Notification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library.Core.BaseClasses;

    public class RecipientRemoveCriteria : CommandBase
    {
        public int RecipientId { get; set; }

        public int UserId { get; set; }
        public int FraudCaseId { get; internal set; }

        public override string GetSettingKey() => "FS_Recipients_Delete";

        public override bool IsValid() => true;
    }
}