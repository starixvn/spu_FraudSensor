namespace Fanex.NAP.FS.Library.Models
{
    public enum AccountStatus
    {
        All,
        Open,
        Suspended,
        Closed,
        CN88_Active,
        CN88_ViewOnly,
        CN88_Suspended,
        CN88_Inactive,
        CN88_Locked
    }

    public enum EvidenceStatus
    {
        All,
        Affected,        
        Clean
    }

    public enum AccountEvidenceStatus
    {
        Clean,
        CentralPoint,
        Flagged
    }

    public enum TransactionHistoryLength
    {
        Hours24 = 1,
        Days7 = 7,
        Days30 = 30,
        Days180 = 180
    }

    public enum ProblemAccountStatus
    {
        Yes,
        No
    }
}