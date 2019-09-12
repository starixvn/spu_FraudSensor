namespace Fanex.NAP.FS.Library.ViewModels.Account.Evidence
{
    using System.Collections.Generic;

    public class RetractedAndExceptionEvidence
    {
        public IEnumerable<RetractedAndExceptionNode> ChainsDirectedRetractedEvidence { get; set; }

        public IEnumerable<RetractedAndExceptionNode> ChainsAffectedRetractedEvidence { get; set; }

        public IEnumerable<RetractedAndExceptionNode> ChainsExceptionEvidence { get; set; }
    }
}