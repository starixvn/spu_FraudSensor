using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.FS.Library;
using Fanex.NAP.FS.Websites.Models;
using System;
using System.Collections.Generic;

namespace Fanex.NAP.FS.Websites.Service
{
    public static class DataService
    {
        public static IEnumerable<T> GetData<T>(CriteriaBase criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<T>(criteria);
            }
        }

        public static DBResult UpdateData(CriteriaBase criteria, string prefix)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var param = criteria.GetSpParams();

                db.ExecuteNonQuery(param);

                var code = param["ErrCode"];

                var intCode = (code is DBNull) ? 0 : Convert.ToInt32(code);

                return new DBResult
                {
                    ErrorCode = intCode,
                    ErrorMsg = ErrorLookup.GetValue(prefix + "_" + intCode),
                    OutParam = param
                };
            }
        }

        public static T GetSingle<T>(CriteriaBase criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<T>(criteria);
            }
        }
    }
}