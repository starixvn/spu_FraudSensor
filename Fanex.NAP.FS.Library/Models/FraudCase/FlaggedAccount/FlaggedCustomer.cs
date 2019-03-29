using System.Xml.Serialization;

namespace Fanex.NAP.FS.Library.Models.FraudCase.FlaggedAccount
{
    public class FlaggedCustomer
    {
        public int CustId { get; set; }

        public string EvidenceCode { get; set; }

        [XmlIgnore]
        public string EvidenceName { get; set; }
    }
}