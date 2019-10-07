namespace Fanex.NAP.FS.Library.ViewModels.Account.AssociatedToMonitor
{
    using System;

    public class AssociatedAccountsToMonitor
    {
        public int AssAccId { get; set; }

        public string AssociatedAccountUsername { get; set; }

        public string AssociatedAccountSite { get; set; }

        public int AssociatedAccountLevel { get; set; }

        public string AssociatedAccountLevelName
        {
            get
            {
                try
                {
                    return Enum.GetName(typeof(UserLevelName), this.AssociatedAccountLevel);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public string AssociatedAccountEvidences { get; set; }

        public string MainAccountUsername { get; set; }

        public string MainAccountSite { get; set; }

        public int MainAccountLevel { get; set; }

        public string MainAccountLevelName
        {
            get
            {
                try
                {
                    return Enum.GetName(typeof(UserLevelName), this.MainAccountLevel);
                }
                catch
                {
                    return string.Empty;
                }
            }        
        }

        public string MainAccountEvidences { get; set; }

        public DateTime FirstAssociatedDate { get; set; }

        public string FirstAssociatedDateToString
        {
            get
            {
                if (this.FirstAssociatedDate != DateTime.MinValue)
                {
                    return this.FirstAssociatedDate.ToString("MMM dd, yyyy");
                }

                return string.Empty;
            }
        }

        public bool IsVerified { get; set; }

        public int VerifiedBy { get; set; }

        public string VerifiedByUserName { get; set; }

        public string VerifiedDate { get; set; }

        public string VerifiedOfView
        {
            get
            {
                if (this.IsVerified && this.VerifiedDate != null)
                {
                    DateTime date = DateTime.Parse(this.VerifiedDate);

                    return this.VerifiedByUserName + ", " + date.ToString("MMM dd, yyyy");
                }

                return string.Empty;
            }
        }

        public string Note { get; set; }
    }
}
