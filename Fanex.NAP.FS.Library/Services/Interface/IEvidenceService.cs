namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.ViewModels.SubscriberAccount.Evidence;

    public interface IEvidenceService
    {
        ChainsToEvidence GetChainsToEvidence(int accountId);

        RetractedEvidenceAndException GetRetractedEvidenceAndException(int accountId);
    }
}