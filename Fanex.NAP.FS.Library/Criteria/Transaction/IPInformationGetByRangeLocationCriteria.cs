namespace Fanex.NAP.FS.Library.Criteria.Transaction
{
    using Fanex.Data.Repository;

    public class IPInformationGetByRangeLocationCriteria : CriteriaBase
    {
        public decimal ip_IP { get; set; }

        public override string GetSettingKey() => "FPS_DC_GetLocationInfobyIP";

        public override bool IsValid() => true;
    }
}