namespace Fanex.NAP.FS.Library.Criteria.SuspiciousInfo
{
    using Fanex.Data.Repository;
    using System;

    public class SuspiciousInfoUpdateCriteria : CriteriaBase
    {
        public int Custid { get; set; }

        public int UserId { get; set; }

        public int FraudCaseId { get; set; }

        public DateTime? EventDate { get; set; }

        public DateTime? SuspiciousDate { get; set; }

        public string TransIds { get; set; }

        public int? ViolationCounter { get; set; }

        public int? TicketCounter { get; set; }

        public DateTime? VerifiedDate { get; set; }

        public int? VerifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "FS_Tbl_SuspiciousInfo_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}