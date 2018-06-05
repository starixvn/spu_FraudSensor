namespace Fanex.NAP.FS.Library.Criteria.FraudCase.ScannedOfficeIP
{
    using Fanex.Data.Repository;

    public class OfficeIPDeleteCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string IPRange { get; set; }

        public int FromNumber { get; set; }

        public int ToNumber { get; set; }

        public override string GetSettingKey() => "FS_Tbl_OfficeIP_Delete";

        public override bool IsValid() => true;
    }
}