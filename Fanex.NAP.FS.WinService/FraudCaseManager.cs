namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.DTO;
    using Fanex.NAP.FS.Library.Models;
    using Fanex.NAP.FS.Library.Services;
    using Fanex.NAP.FS.WinService.Executor;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;

    public class FraudCaseManager : IFraudCaseManager
    {
        private readonly IFraudCaseService service;

        private readonly IDictionary<FraudCaseId, Type> _lookup;

        public FraudCaseManager(IFraudCaseService service)
        {
            this.service = service;

            _lookup = 
                typeof(FraudCaseManager).Assembly.GetTypes()
                 .Where(t => typeof(FraudCaseExecutor).IsAssignableFrom(t) && !t.IsAbstract && t.GetCustomAttribute<FraudCaseAttribute>() != null)
                .ToDictionary(t => t.GetCustomAttribute<FraudCaseAttribute>().FraudCase);
        }

        public FraudCase GetCase(FraudCaseId id)
        {
            return service.GetByFraudCaseId((int)id);
        }

        public IEnumerable<FraudCase> GetCases()
        {
            return service.Get(null, 1, int.Parse(ConfigurationManager.AppSettings["Environment"]));
        }

        public IFraudCaseExecutor GetExecutor(FraudCase fraudCase)
        {
            var id = (FraudCaseId)fraudCase.FraudCaseId;

            if (_lookup.ContainsKey(id))
            {
                return Activator.CreateInstance(_lookup[id], fraudCase) as FraudCaseExecutor;
            }

            return new GhostExecutor(fraudCase);
        }
    }
}