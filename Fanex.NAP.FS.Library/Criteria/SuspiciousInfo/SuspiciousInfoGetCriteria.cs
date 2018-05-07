namespace Fanex.NAP.FS.Library.Criteria.SuspiciousInfo
{
    using Fanex.Data.Repository;

    public class SuspiciousInfoGetCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Tbl_SuspiciousInfo_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}