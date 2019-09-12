namespace Fanex.NAP.FS.Library.Services
{
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.ViewModels.Account.Evidence;
    using Fanex.NAP.FS.Library.ViewModels.Evidence;
    using System.Collections.Generic;

    public interface IEvidenceService
    {
        ChainsToEvidence GetChainsToEvidence(ChainsToEvidenceFilter filter);

        RetractedAndExceptionEvidence GetRetractedAndExceptionEvidence(RetractedAndExceptionEvidenceFilter filter);

        IEnumerable<Evidence> GetAllEvidences();

        IEnumerable<Evidence> GetEvidencesByAccount(int accountId);

        DBResult InsertEvidenceGroup(string groupName);

        DBResult InsertEvidence(int groupId, string evidenceName);

        DBResult DeleteEvidence(int evidenceId);

        DBResult UpdateEvidenceByAccount(EvidenceByAccount evidenceByAccount);
    }
}