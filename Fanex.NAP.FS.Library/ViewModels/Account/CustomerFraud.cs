namespace Fanex.NAP.FS.Library.ViewModels.Account
{
    using Fanex.NAP.FS.Library.Models;
    using System;

    public class CustomerFraud
    {
        public int custid { get; set; }

        public string username { get; set; }

        public bool reject { get; set; }

        public bool closed { get; set; }

        public int danger { get; set; }

        public int danger2 { get; set; }

        public int danger3 { get; set; }

        public bool autosuspend { get; set; }

        public bool autosuspend2 { get; set; }

        public int Abnormal { get; set; }

        public bool ProblemAccount { get; set; }

        public AccountStatus GetStatus()
        {
            var status = AccountStatus.Active;

            if (reject)
            {
                status = AccountStatus.Suspended;
            }

            if (closed)
            {
                status = AccountStatus.Closed;
            }

            return status;
        }

        public string GetStatusName()
        {
            return Enum.GetName(typeof(AccountStatus), this.GetStatus());
        }

        public bool CompareStatus(AccountStatus status)
        {
            if (status == AccountStatus.All)
            {
                return true;
            }

            var currentStatus = GetStatus();

            return currentStatus.Equals(status);
        }
    }
}