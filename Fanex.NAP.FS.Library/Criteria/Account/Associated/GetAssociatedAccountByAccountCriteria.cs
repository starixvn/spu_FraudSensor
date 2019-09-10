namespace Fanex.NAP.FS.Library.Criteria.Account.Associated
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Core.BaseClasses;

    public class GetAssociatedAccountByAccountCriteria : CommandBase
    {
        public string custIds { get; set; }

        public int rootAccountId { get; set; }

        public int userId { get; set; }

        public bool isMaster { get; set; }

        [SpParam(Ignored = true)]
        public override int ErrCode { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_DC_Rpt_GetAssociatedAccountByAccount";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}