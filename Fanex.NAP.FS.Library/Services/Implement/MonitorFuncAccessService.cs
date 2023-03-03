namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.Data.Repository;
    using Fanex.NAP.FS.Library.Core.Consts;
    using Fanex.NAP.FS.Library.Criteria.MonitorFuncAccess;
    using Fanex.NAP.FS.Library.Models;

    public interface IMonitorFuncAccessService
    {
        DBResult StatsAccessmentInsert(SystemId systemId, TrackingFunctionId funcId, int userId);
    }

    public class MonitorFuncAccessService : IMonitorFuncAccessService
    {
        private readonly IDynamicRepository _repository;

        public MonitorFuncAccessService()
        {
            _repository = new DynamicRepository();
        }

        public DBResult StatsAccessmentInsert(SystemId systemId, TrackingFunctionId funcId, int userId)
        {
            var criteria = new StatsAccessmentInsertCriteria
            {
                SystemId = (long)systemId,
                FunctionId = (int)funcId,
                UserId = userId
            };

            _repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : 1,
                ErrorMsg = criteria.ErrorMessage
            };
        }
    }
}
