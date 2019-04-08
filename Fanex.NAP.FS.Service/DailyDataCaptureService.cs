using Fanex.Data;
using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Service
{
    public class DailyDataCaptureService : IExecutorService
    {
        public IEnumerable<T> PreCondition<T>()
        {
            throw new NotImplementedException();
        }

        public void Rule(string matchIds, string param1)
        {
            throw new NotImplementedException();
        }

        public void Rule(string matchIds)
        {
            throw new NotImplementedException();
        }

        public void Rule()
        {
            var criteria = new DailyDataCaptureRuleCriteria();            

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var param = criteria.GetSpParams();

                db.ExecuteNonQuery(param);
            }
        }
    }
}
