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
            //using (var db = new ObjectDb(criteria.GetSettingKey()))
            //{
            //    return db.Query<T>(criteria);
            //}

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<T>(criteria);
            }
        }

        public static void Execute(CriteriaBase criteria)
        {
            //using (var db = new ObjectDb(criteria.GetSettingKey()))
            //{
            //    var param = criteria.GetSpParams();
            //    db.ExecuteNonQuery(param);
            //}

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
            }
        }

        public static DBResult UpdateData(CommandBase criteria, string prefix)
        {
            var repository = new DynamicRepository();

            repository.Execute(criteria);

            var intCode = (criteria.ErrCode is DBNull) ? 0 : Convert.ToInt32(criteria.ErrCode);

            return new DBResult
            {
                ErrorCode = intCode,
                ErrorMsg = ErrorLookup.GetValue(prefix + "_" + intCode)
            };           
        }

        public static T GetSingle<T>(CriteriaBase criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<T>(criteria);
            }
        }       
    }
}