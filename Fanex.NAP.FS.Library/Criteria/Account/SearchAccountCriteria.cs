namespace Fanex.NAP.FS.Library.Criteria.Account
{
    using Fanex.Data.Repository;

    public class SearchAccountCriteria : CriteriaBase
    {
        public string ip_SubscriberIDs { get; set; }

        public string ip_Username { get; set; }        

        public override string GetSettingKey() => "FPS_DC_SearchAccount";

        public override bool IsValid() => true;
    }
}