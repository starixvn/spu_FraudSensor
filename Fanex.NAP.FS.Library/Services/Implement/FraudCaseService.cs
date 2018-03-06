namespace Fanex.NAP.FS.Library.Services
{
    using Core;
    using Criteria;
    using DTO;
    using System.Collections.Generic;

    public class FraudCaseService : FSBaseService, IFraudCaseService
    {
        public IEnumerable<FraudCase> GetAll()
        {
            return GetData<FraudCase>(new FraudCaseGetAllCriteria());
        }

        public FraudCase GetByFraudCaseId(int fraudCaseId)
        {
            return GetSingle<FraudCase>(new FraudCaseGetByIdCriteria
            {
                FraudCaseId = fraudCaseId
            });
        }
    }
}
