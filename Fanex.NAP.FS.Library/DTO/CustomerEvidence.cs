namespace Fanex.NAP.FS.Library.DTO
{
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;

    [XName("FlaggedCustomer")]
    public class CustomerEvidence
    {
        public int CustId { get; set; }

        public string EvidenceCode { get; set; }

        public string EvidenceCodeForDisplay
        {
            get
            {
                if (this.EvidenceCode == Consts.FPS_EVIDENCE_CODE_SEIZE_BET_99_100)
                {
                    return this.EvidenceCode.Replace(Consts.FPS_EVIDENCE_CODE_SEIZE_BET_99_100, Consts.FPS_EVIDENCE_CODE_SEIZE_BET_100);
                }

                return this.EvidenceCode;
            }
        }

        public string EvidenceName { get; set; }
    }
}