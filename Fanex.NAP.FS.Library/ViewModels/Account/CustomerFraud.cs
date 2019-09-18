namespace Fanex.NAP.FS.Library.ViewModels.Account
{
    using Fanex.NAP.FS.Library.Models;
    using System;
    using System.Linq;

    public class CustomerFraud
    {
        public int custid { get; set; }

        public string username { get; set; }

        /// <summary>
        /// 1: Inactive, 2: Suspended, 3: ViewOnly, 4: Active
        /// -1: customer isn't CN88
        /// </summary>
        public int cn88status { get; set; }

        /// <summary>
        /// suspend (reject=true) only for MBC
        /// </summary>
        public bool reject { get; set; }

        /// <summary>
        /// CN88: locked (closed=1) || MBC: disabled (closed=1) active (closed=0)
        /// </summary>
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
            if (cn88status == -1)
            {
                return GetAccountStatusForMBC();
            }

            return GetAccountStatusForCN88();
        }

        public bool CompareStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return true;
            }

            var listStatus = status.Split(',');

            var currentStatus = Enum.GetName(typeof(AccountStatus), GetStatus());

            return listStatus.Contains(currentStatus);
        }

        public string GetStatusName()
        {
            return Enum.GetName(typeof(AccountStatus), this.GetStatus());
        }

        private AccountStatus GetAccountStatusForMBC()
        {
            if (this.reject)
            {
                return AccountStatus.Suspended;
            }

            if (this.closed)
            {
                return AccountStatus.Closed;
            }

            return AccountStatus.Open;
        }

        private AccountStatus GetAccountStatusForCN88()
        {
            switch (this.cn88status)
            {
                case 1:
                    return AccountStatus.CN88_Inactive;
                case 2:
                    return AccountStatus.CN88_Suspended;
                case 3:
                    return AccountStatus.CN88_ViewOnly;
                default:
                    if (this.closed)
                    {
                        return AccountStatus.CN88_Locked;
                    }
                    return AccountStatus.CN88_Active;
            }
        }
    }
}