namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using System;
    using System.Collections.Generic;

    public class DailyDataCaptureService : IExecutorService
    {
        public IEnumerable<T> PreCondition<T>()
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
