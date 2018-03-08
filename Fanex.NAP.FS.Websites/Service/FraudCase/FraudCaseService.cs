using Fanex.NAP.FS.Websites.Criteria.FraudCase;
using Fanex.NAP.FS.Websites.Models;

namespace Fanex.NAP.FS.Websites.Service.FraudCase
{
    public static class FraudCaseService
    {
        public static bool CheckActiveFraudCase(FraudCaseId id)
        {
            var criteria = new GetIsActiveCriteria
            {
                FraudCaseId = (int)id
            };

            var result = DataService.GetSingle<dynamic>(criteria);

            return result.IsActive;
        }

        public static void ChangeFraudCaseStatus(FraudCaseId id, int userId)
        {
            var criteria = new UpdateFraudCaseStatusCriteria
            {
                FraudCaseId = (int)id,
                UserId = userId
            };

            DataService.UpdateData(criteria);
        }
    }
}