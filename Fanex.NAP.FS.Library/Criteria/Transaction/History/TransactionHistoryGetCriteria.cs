namespace Fanex.NAP.FS.Library.Criteria.Transaction.History
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using System;
    using System.Collections;

    public class TransactionHistoryGetCriteria : CommandBase
    {
        public int ip_AccountId { get; set; }

        public DateTime ip_FromDate { get; set; }

        public int ip_Skip { get; set; }

        public int ip_Limit { get; set; }

        [SpParam(Ignored = true)]
        public override int ErrCode { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_GetTransactionByAccount";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}