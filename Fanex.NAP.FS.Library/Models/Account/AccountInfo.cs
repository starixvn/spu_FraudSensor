namespace Fanex.NAP.FS.Library.Models.Account
{
    using System;

    public class AccountInfo
    {
        public int AccountId { get; set; }

        public string UserName { get; set; }

        public string UserName2 { get; set; }

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