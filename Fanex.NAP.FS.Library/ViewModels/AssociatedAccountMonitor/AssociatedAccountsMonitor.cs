namespace Fanex.NAP.FS.Library.ViewModels.AssociatedAccountMonitor
{
    using System;

    public class AssociatedAccountsMonitor
    {
        public int MainAccountCustId { get; set; }

        public string MainAccountUsername { get; set; }

        public string MainAccountSite { get; set; }

        public int MainAccountLevel { get; set; }

        public string MainAccountLevelName
        {
            get
            {
                try
                {
                    return Enum.GetName(typeof(RoleLevel), this.MainAccountLevel);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public string MainAccountEvidences { get; set; }  

        public int NoOfAssociations { get; set; }

        public int NoOfVerified { get; set; }
    }
}