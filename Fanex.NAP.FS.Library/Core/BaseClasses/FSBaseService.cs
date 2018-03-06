namespace Fanex.NAP.FS.Library.Core
{
    using Data;
    using Data.Repository;
    using NAP.Core.Command;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class FSBaseService
    {
        public IEnumerable<T> GetData<T>(CriteriaBase criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<T>(criteria);
            }
        }

        public T GetSingle<T>(CriteriaBase criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var result = db.Query<T>(criteria);

                return result.FirstOrDefault();
            }
        }

        public ICommandResult UpdateData(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var param = criteria.GetSpParams();

                db.ExecuteNonQuery(param);

                return GetCommandResult(param);
            }
        }

        private CommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errcode"];
            var errMsg = hash["errmsg"];

            return new CommandResult
            {
                Code = (errCode is DBNull) ? 0 : Convert.ToInt32(errCode),
                Message = (errMsg is DBNull) ? string.Empty : Convert.ToString(errMsg)
            };
        }
    }
}
