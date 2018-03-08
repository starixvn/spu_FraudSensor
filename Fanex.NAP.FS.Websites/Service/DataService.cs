using Fanex.Data;
using Fanex.Data.Repository;
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

        public static void UpdateData(CriteriaBase criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
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