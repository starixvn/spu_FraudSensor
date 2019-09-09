namespace Fanex.NAP.FS.Library.Criteria.Account.GeneralInformation
{
    public class GetEvidenceByTargetIdCriteria
    {
        public int targetId { get; set; }

        public bool objType { get; set; }

        public string subIdList { get; set; }

        public bool isMaster { get; set; }
    }
}