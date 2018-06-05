namespace Fanex.NAP.FS.Library.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class UserMessageGetCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public UserMessageGetCriteria(int custId)
        {
            CustId = custId;
        }

        public override string GetSettingKey()
        {
            return "FS_Tbl_UserMessage_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}