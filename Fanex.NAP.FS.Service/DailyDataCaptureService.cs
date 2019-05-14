using Fanex.Data.Repository;

namespace Fanex.NAP.FS.Service
{
    using Fanex.Data;
    using Fanex.NAP.FS.Library.Criteria.FraudSensorTask;
    using System;
    using System.Collections.Generic;

    public class DailyDataCaptureService
    {
        public void Rule()
        {
            var repository = new DynamicRepository();
            var criteria = new DailyDataCaptureRuleCommand();

            repository.Execute(criteria);
        }
    }
}
