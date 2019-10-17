namespace Fanex.NAP.FS.Library.ViewModels.Evidence
{
    public class InformationEvidenceByAccountHistory
    {
        public string CreatedDate { get; set; }

        public string Action { get; set; }

        public string Creator { get; set; }

        public string EvidenceTypeCode { get; set; }

        public string EvidenceTypeName { get; set; }

        public string EvidenceType
        {
            get { return this.EvidenceTypeCode + " " + this.EvidenceTypeName; }        
        }

        public string Comment { get; set; }
    }
}
