namespace Fanex.NAP.FS.Library.ViewModels.AssociatedAccountMonitor
{
    using System;

    public class AssociatedAccountsMonitorDetail
    {
        public int AssAccId { get; set; }

        public int AssociatedCustId { get; set; }

        public string AssociatedCustUsername { get; set; }

        public string AssociatedCustSite { get; set; }

        public int AssociatedCustLevel { get; set; }

        public string AssociatedCustLevelName
        {
            get
            {
                try
                {
                    return Enum.GetName(typeof(RoleLevel), this.AssociatedCustLevel);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public string AssociatedCustEvidences { get; set; }       

        public DateTime FirstAssociatedDate { get; set; }

        public string FirstAssociatedDateToString
        {
            get
            {
                if (this.FirstAssociatedDate != DateTime.MinValue)
                {
                    return this.FirstAssociatedDate.ToString("MMM dd, yyyy");
                }

                return null;
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

                    return this.VerifiedByUserName + "; " + date.ToString("MMM dd, yyyy");
                }

                return null;
            }
        }

        public string Note { get; set; }
    }
}