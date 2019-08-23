namespace Fanex.NAP.FS.Library.Models.Account
{
    using Fanex.NAP.FS.Library.Models.Subscriber;
    using System;

    public class Account
    {
        public int Id { get; set; }

        public int CustId { get; set; }

        public string UserName { get; set; }

        public string UserName2 { get; set; }

        public Subscriber Subscriber { get; set; }

        public string Status { get; set; }

        public string FraudStatus { get; set; }

        public DateTime FirstLogin { get; set; }

        public DateTime LastLogin { get; set; }

        public int FrequencyLogin { get; set; }

        public int AssociatedAccounts { get; set; }

        public int AssociatedDevices { get; set; }

        public int SharedDevices { get; set; }

        public int NoSharedDevices { get; set; }

        public int AccountExceptions { get; set; }

        public string Evidence { get; set; }

        public bool ProblemAccount { get; set; }

        public int Abnormal { get; set; }

        public int DangerousLevel { get; set; }

        public string AutoSuspend { get; set; }

        public string CommitedFraudCase { get; set; }
    }
}