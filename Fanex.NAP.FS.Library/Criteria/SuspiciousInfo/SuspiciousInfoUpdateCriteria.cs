namespace Fanex.NAP.FS.Library.Criteria.SuspiciousInfo
{
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using System;

    public class SuspiciousInfoUpdateCriteria : CommandBase
    {
        public int UserId { get; set; }

        public int CustId { get; set; }

        public int? MatchId { get; set; }

        public int FraudCaseId { get; set; }

        public DateTime? EventDate { get; set; }

        public DateTime? SuspiciousDate { get; set; }

        public string TransIds { get; set; }

        public bool? IsVerified { get; set; }

        public int? ViolationCounter { get; set; }

        public int? TicketCounter { get; set; }

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