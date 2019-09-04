namespace Fanex.NAP.FS.Library.Models.Account
{
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using System;

    public class AccountGeneralInformation
    {
        public int AccountId { get; set; }

        public int CustId { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public Subscriber Subscriber { get; set; }

        public int AccountStatus { get; set; }

        public string Status { get; set; }

        public FraudStatus FraudStatus
        {
            get
            {
                if (this.AccountStatus > 0)
                {
                    return FraudStatus.Fraud;
                }

                return FraudStatus.Clean;
            }
        }

        public DateTime FirstLogin { get; set; }

        public DateTime LastLogin { get; set; }

        public int FrequencyLogin { get; set; }

        public int AccountAge { get; set; }

        public int AssociatedAccounts { get; set; }

        public int AssociatedDevices { get; set; }

        public int SharedDevices { get; set; }

        public int NoSharedDevices { get; set; }

        public int AccountExceptions { get; set; }

        public string Evidence { get; set; }

        public string ProblemAccount { get; set; }

        public int Abnormal { get; set; }

        public int DangerousOriginal { get; set; }

        public int DangerousNonLive { get; set; }

        public int DangerousOtherSports { get; set; }

        public string AutoSuspend { get; set; }        

        public string CommitedFraudCase { get; set; }

        public string FirstLoginToString
        {
            get {

                if (FirstLogin == DateTime.MinValue)
                {
                    return string.Empty;
                }

                return FirstLogin.ToString(Consts.FormatDateToString_MMMddyyyy);
            }        
        }

        public string LastLoginToString
        {
            get
            {

                if (LastLogin == DateTime.MinValue)
                {
                    return string.Empty;
                }

                return LastLogin.ToString(Consts.FormatDateToString_MMMddyyyy);
            }
        }
    }
}