namespace Fanex.NAP.FS.Library.ViewModels.Evidence
{
    using System.Collections.Generic;

    public class InformationEvidenceByAccount
    {
        public int AccountId { get; set; }

        public string UserName { get; set; }

        public string EvidenceTypeCode { get; set; }

        public string EvidenceTypeName { get; set; }

        public string CreatedBy { get; set; }

        public string Comment { get; set; }

        public List<InformationEvidenceHistoryByAccount> History { get; set; }
    }
}