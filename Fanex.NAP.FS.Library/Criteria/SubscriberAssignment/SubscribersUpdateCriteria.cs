namespace Fanex.NAP.FS.Library.Criteria.SubscriberAssignment
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Core.BaseClasses;

    public class SubscribersUpdateCriteria : CommandBase
    {
        public bool ip_isMasterSub { get; set; }

        public int ip_userId { get; set; }

        public string ip_subscriberIds { get; set; }

        public int ip_createdBy { get; set; }

        [SpParam(Ignored = true)]
        public override int ErrCode { get; set; }

        public override string GetSettingKey() => "FPS_AD_InsertSubscriberByUser";

        public override bool IsValid() => true;
    }
}