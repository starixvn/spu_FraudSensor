namespace Fanex.NAP.FS.Library.ViewModels.Account.Associated
{
    using Fanex.NAP.FS.Library.Core.Consts;
    using System;   

    public class AssociatedAccountByAccount
    {
        public int AccountID { get; set; }

        public int CustID { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public string SubscriberName { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime FirstLogin { get; set; }

        public DateTime AssociatedWithRootAccountOn { get; set; }       

        public int TotalLoginWithRootAccount { get; set; }

        public int EvidenceLevel { get; set; }

        public string Status { get; set; }

        public string LastLoginToString
        {
            get { return GetDateToString(LastLogin, Consts.FormatDateToString_MMMddyyyy); }
        }

        public string FirstLoginToString
        {
            get { return GetDateToString(FirstLogin, Consts.FormatDateToString_MMMddyyyy); }
        }

        public string AssociatedWithRootAccountOnToString
        {
            get { return GetDateToString(AssociatedWithRootAccountOn, Consts.FormatDateToString_MMMddyyyy); }
        }

        public static string GetDateToString(DateTime dateTime, string formatString)
        {
            if (dateTime == DateTime.MinValue)
            {
                return string.Empty;
            }

            return dateTime.ToString(formatString);
        }
    }
}