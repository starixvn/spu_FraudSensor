namespace Fanex.NAP.FS.Library.Services
{
    using DTO;
    using System.Collections.Generic;

    public interface IFraudCaseService
    {
        FraudCase GetByFraudCaseId(int fraudCaseId);

        IEnumerable<FraudCase> GetAll();
    }
}
