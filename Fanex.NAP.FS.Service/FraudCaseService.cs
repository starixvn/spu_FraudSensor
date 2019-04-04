using Fanex.Data;
using Fanex.NAP.FS.Core;
using Fanex.NAP.FS.Library.Criteria;
using Fanex.NAP.FS.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Service
{
   public class FraudCaseService
    {
        public IEnumerable<FraudCase> Get(int? isInternal, int? isActive, int environment)
        {
            var criteria = new FraudCaseGetCriteria(isInternal, isActive, environment);;
            
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {                
                return db.Query<FraudCase>(criteria);
            }
        }

        public FraudCase GetByFraudCaseId(int fraudCaseId, int environment)
        {
            var criteria = new FraudCaseGetByIdCriteria()
            {
                FraudCaseId = fraudCaseId,
                Environment = environment
            };

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {                
                return db.QueryEntity<FraudCase>(criteria);
            }
        }

        /// <summary>
        /// Use this for Micro service
        /// </summary>
        /// <returns></returns>
        public FraudCase GetFraudCaseFromConfig()
        {
            var fraudCase = new FraudCase()
            {
                FraudCaseId = AppSetting.FraudCaseId,
                FraudCaseName = AppSetting.FraudCaseName,
                FraudCaseInterval = AppSetting.FraudCaseInterval,
                FraudCaseDesc = AppSetting.FraudCaseDesc,
                IsActive = AppSetting.IsActive,
                IsTest = AppSetting.IsTest
            };

            return fraudCase;
        }
    }
}
