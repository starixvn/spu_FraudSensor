namespace Fanex.NAP.FS.Library.Models.Account
{
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;

    public class ChainNode
    {
        public Evidence Evidence { get; set; }

        public string NodeType { get; set; }

        public int NodeIdCurrent { get; set; }

        public string NodeNameCurrent { get; set; }

        public int NodeIdParrent { get; set; }

        public string NodeNameParent { get; set; }
    }
}