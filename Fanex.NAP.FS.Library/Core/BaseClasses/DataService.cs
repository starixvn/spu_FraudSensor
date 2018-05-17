namespace Fanex.NAP.FS.Library.Core.BaseClasses
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library;
    using Fanex.NAP.FS.Library.Models;
    using System;
    using System.Collections.Generic;

    public static class DataService
    {
        public static IEnumerable<T> GetData<T>(CriteriaBase criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<T>(criteria);
            }
        }

        public static void Execute(CriteriaBase criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var param = criteria.GetSpParams();

                db.ExecuteNonQuery(param);
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

        public static void QueryMultiple(CriteriaBase criteria, params Action<IMultiResultReader>[] extractors)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            using (var query = db.QueryMultiple(criteria))
            {
                for (int i = 0; i < extractors.Length; i++)
                {
                    extractors[i](query);
                }
            }
        }

        public static Tuple<IEnumerable<T>, IEnumerable<R>> QueryMultiple<T, R>(CriteriaBase criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            using (var query = db.QueryMultiple(criteria))
            {
                return Tuple.Create(query.Read<T>(), query.Read<R>());
            }
        }

        public static Tuple<IEnumerable<T>, IEnumerable<R>, IEnumerable<U>> QueryMultiple<T, R, U>(CriteriaBase criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            using (var query = db.QueryMultiple(criteria))
            {
                return Tuple.Create(query.Read<T>(), query.Read<R>(), query.Read<U>());
            }
        }
    }
}