namespace Fanex.NAP.FS.Library.Services
{
    using System.Collections.Generic;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.ViewModels.Account.Evidence;
    using Fanex.NAP.FS.Library.ViewModels.Account.Evidence.Exception;
    using Fanex.NAP.FS.Library.ViewModels.Evidence;    

    public interface IEvidenceService
    {
        ChainsToEvidence GetChainsToEvidence(ChainsToEvidenceFilter filter);

        DBResult AddExceptionEvidence(ExceptionEvidenceAdd exceptionEvidenceAdd);

        RetractedAndExceptionEvidence GetRetractedAndExceptionEvidence(RetractedAndExceptionEvidenceFilter filter);

        IEnumerable<Evidence> GetAllEvidences();

        IEnumerable<Evidence> GetEvidencesByAccount(int accountId);

        DBResult InsertEvidenceGroup(string groupName);

        DBResult InsertEvidence(int groupId, string evidenceName);

        DBResult DeleteEvidence(int evidenceId);

        DBResult PlaceEvidenceByAccount(PlaceEvidenceByAccount evidenceByAccount);

        IEnumerable<InformationEvidenceByAccount> GetInformationEvidenceByAccount(InformationEvidenceByAccountFilter filter);
    }
}