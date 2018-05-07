namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor
{
    using Fanex.Data.Repository;

    public class GetTicketByIPCriteria : CriteriaBase
    {
        public string IP { get; set; }

        public override string GetSettingKey() => "FS_Rpt_Tickets_GetByIP";

        public override bool IsValid() => true;
    }
}