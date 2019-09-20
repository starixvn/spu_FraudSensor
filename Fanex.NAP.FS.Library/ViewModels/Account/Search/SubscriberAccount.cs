namespace Fanex.NAP.FS.Library.ViewModels.Account.Search
{
    public class SubscriberAccount
    {
        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public int AccountID { get; set; }

        public int CustID { get; set; }

        public string UserName { get; set; }

        public string UserName2 { get; set; }

        public string FirstLoginName { get; set; }

        public bool IsCustomerExisted
        {
            get { return !string.IsNullOrEmpty(this.UserName) && !string.IsNullOrEmpty(this.UserName2); }
        }

        public string GetUserName()
        {
            if (!string.IsNullOrEmpty(this.UserName) && !string.IsNullOrEmpty(this.UserName2))
            {
                return this.UserName;
            }

            return this.FirstLoginName;
        }
    }
}