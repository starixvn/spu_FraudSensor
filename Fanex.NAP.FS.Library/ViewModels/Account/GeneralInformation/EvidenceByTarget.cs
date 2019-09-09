namespace Fanex.NAP.FS.Library.ViewModels.Account.GeneralInformation
{
    public class EvidenceByTarget
    {
        public int TargetEvidenceID { get; set; }

        public int TargetID { get; set; }

        public int TargetLevel { get; set; }

        public bool TargetType { get; set; }

        public int CentralPointID { get; set; }

        public bool CentralPointType { get; set; }

        public string EvidenceTypeCode { get; set; }

        public string SubscriberName { get; set; }

        public bool IsCreatedByMaster { get; set; }
    }
}