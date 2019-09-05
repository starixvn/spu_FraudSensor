namespace Fanex.NAP.FS.Library.Criteria.Transaction
{
    using Fanex.Data.Repository;

    public class IPInformationGetByRangeLocationCriteria : CriteriaBase
    {
        public int ip_IPRangeLocationID { get; set; }

        public override string GetSettingKey() => "FPS_DC_GetLocationInfobyIP";

        public override bool IsValid() => true;
    }
}