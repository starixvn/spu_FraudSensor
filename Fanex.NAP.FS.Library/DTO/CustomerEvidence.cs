namespace Fanex.NAP.FS.Library.DTO
{
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;

    [XName("FlaggedCustomer")]
    public class CustomerEvidence
    {
        public int CustId { get; set; }

        public string EvidenceCode { get; set; }

        public string EvidenceName { get; set; }
    }
}