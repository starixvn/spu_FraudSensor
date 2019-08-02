namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.ViewModels.Account.Evidence;

    public interface IEvidenceService
    {
        ChainsToEvidence GetChainsToEvidence(int accountId);

        RetractedEvidenceAndException GetRetractedEvidenceAndException(int accountId);
    }
}