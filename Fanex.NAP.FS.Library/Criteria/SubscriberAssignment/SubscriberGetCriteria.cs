namespace Fanex.NAP.FS.Library.Criteria.SubscriberAssignment
{
    using Fanex.Data.Repository;

    public class SubscriberGetCriteria : CriteriaBase
    {
        public long UserId { get; set; }

        public override string GetSettingKey() => "";

        public override bool IsValid() => true;

    }
}