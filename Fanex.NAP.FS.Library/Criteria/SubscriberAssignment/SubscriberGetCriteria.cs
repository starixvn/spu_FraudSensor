namespace Fanex.NAP.FS.Library.Criteria.SubscriberAssignment
{
    using Fanex.Data.Repository;

    public class SubscriberGetCriteria : CriteriaBase
    {
        public long ip_userId { get; set; }

        public override string GetSettingKey() => "FPS_AD_GetAllSubscribers";

        public override bool IsValid() => true;

    }
}