namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor
{
    using Fanex.Data.Repository;
    using System;

    public class GetSuspiciousCustIDsByDateCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_MatchMonitor_GetSuspiciousCustIDsByDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}