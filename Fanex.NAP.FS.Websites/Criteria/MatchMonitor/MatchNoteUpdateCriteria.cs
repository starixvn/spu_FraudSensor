using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Websites.Criteria.MatchMonitor
{
    public class MatchNoteUpdateCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int MatchId { get; set; }

        public string Notes { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Rpt_MatchMonitorNotes_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}