namespace Fanex.NAP.FS.Library.Criteria.Account.Evidence
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Core.BaseClasses;

    public class InsertExceptionEvidenceCriteria : CommandBase
    {
        public int ip_MainAccountId { get; set; }

        public int ip_RootAccountId { get; set; }

        public int ip_SubscriberId { get; set; }

        public string ip_Remark { get; set; }

        public int ip_CreatedBy { get; set; }

        [SpParam(Ignored = true)]
        public override int ErrCode { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_InsertException";
        }

        public override bool IsValid() => true;
    }
}