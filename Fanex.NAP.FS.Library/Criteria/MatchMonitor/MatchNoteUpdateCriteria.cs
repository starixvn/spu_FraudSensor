namespace Fanex.NAP.FS.Library.Criteria.MatchMonitor
{
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library.Core.BaseClasses;

    public class MatchNoteUpdateCriteria : CommandBase
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