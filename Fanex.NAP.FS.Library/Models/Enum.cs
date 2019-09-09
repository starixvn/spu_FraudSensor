namespace Fanex.NAP.FS.Library.Models
{
    public enum AccountStatus
    {
        All,
        Active,
        Suspended,
        Closed
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
}