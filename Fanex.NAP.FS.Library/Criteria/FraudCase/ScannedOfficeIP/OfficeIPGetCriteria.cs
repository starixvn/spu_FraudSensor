namespace Fanex.NAP.FS.Library.Criteria.FraudCase.ScannedOfficeIP
{
    using Fanex.Data.Repository;

    public class OfficeIPGetCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "FS_Tbl_OfficeIP_GetAll";

        public override bool IsValid() => true;
    }
}