namespace Fanex.NAP.FS.Library.Criteria.Notification
{
    using Fanex.Data.Repository;

    public class UserMessageUnreadCounterGetCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public UserMessageUnreadCounterGetCriteria(int custId)
        {
            CustId = custId;
        }

        public override string GetSettingKey()
        {
            return "FS_Tbl_UserMessage_GetUnreadCount";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}