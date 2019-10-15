namespace Fanex.NAP.FS.Library.Models.Account
{
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Core.Resources;
    using System;

    public class AccountGeneralInformation
    {
        public int AccountId { get; set; }

        public int CustId { get; set; }

        public string FirstLoginName { get; set; }

        public string Username { get; set; }

        public string Username2 { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public AccountStatus AccountStatus { get; set; }

        public string AccountStatusName {
            get { return Enum.GetName(typeof(AccountStatus), this.AccountStatus); }            
        }

        public AccountEvidenceStatus AccountEvidenceStatus { get; set; }

        public EvidenceStatus EvidenceStatus
        {
            get
            {
                if (!string.IsNullOrEmpty(this.EvidenceTypeCodes))
                {
                    return EvidenceStatus.Affected;
                }

                return EvidenceStatus.Clean;
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

        public string EvidenceTypeCodes { get; set; }

        public ProblemAccountStatus ProblemAccountStatus { get; set; }

        public string ProblemAccount
        {
            get { return Enum.GetName(typeof(ProblemAccountStatus), this.ProblemAccountStatus); }        
        }

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

        public string AccountStatusToString
        {
            get
            {
                switch (this.AccountStatus)
                {                  
                    case AccountStatus.Open:
                        return ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_OPEN);                        
                    case AccountStatus.Suspended:
                        return ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_SUSPENDED);
                    case AccountStatus.Closed:
                        return ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CLOSED);
                    case AccountStatus.CN88_Active:
                        return ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CN88_ACTIVE);
                    case AccountStatus.CN88_ViewOnly:
                        return ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CN88_VIEW_ONLY);
                    case AccountStatus.CN88_Suspended:
                        return ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CN88_SUSPENDED);
                    case AccountStatus.CN88_Inactive:
                        return ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_CN88_INACTIVE);                   
                    default:
                        return ResourcesManager.GetValue(ConstsResources.ACCOUNT_STATUS_NOTYETDEPOSIT);
                }
                
            }
        }
    }
}