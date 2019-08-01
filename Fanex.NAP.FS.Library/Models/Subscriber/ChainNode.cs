namespace Fanex.NAP.FS.Library.Models.Subscriber
{
    using Fanex.NAP.FS.Library.Models.MatchMonitor.FraudCaseDetails;

    public class ChainNode
    {
        public Evidence Evidence { get; set; }

        public string NodeCurrent { get; set; }

        public string NodeParent { get; set; }
    }
}