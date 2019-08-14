namespace Fanex.NAP.FS.Library.Criteria.Transaction.History
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using System;
    using System.Collections;

    public class TransactionHistoryGetByCustIdCriteria : CommandBase
    {
        public int ip_accountId { get; set; }

        public int ip_historyRange { get; set; }
   
        [SpParam(Ignored = true)]
        public override int ErrCode { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_GetTransactionHistoryByCustId";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}