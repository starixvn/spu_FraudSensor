#pragma warning disable S3963 // "static" fields should be initialized inline

namespace Fanex.NAP.FS.WinService
{
    using Fanex.NAP.FS.Library.Core.BaseClasses;
    using Fanex.NAP.FS.Library.Models;
    using Library.DTO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class FraudCaseFactory
    {
        private static readonly IDictionary<FraudCaseId, Type> _lookup;

        public static FraudCaseExecutor GetExecutor(FraudCase fraudCase)
        {
            var id = (FraudCaseId)fraudCase.FraudCaseId;

            if (_lookup.ContainsKey(id))
            {
                return Activator.CreateInstance(_lookup[id], fraudCase) as FraudCaseExecutor;
            }

            return new GhostExecutor(fraudCase);
        }

        static FraudCaseFactory()
        {
            var assembly = typeof(FraudCaseFactory).Assembly;

            _lookup =
                 assembly
                 .GetTypes()
                 .Where(t => typeof(FraudCaseExecutor).IsAssignableFrom(t) && !t.IsAbstract && t.GetCustomAttribute<FraudCaseAttribute>() != null)
                 .ToDictionary(t => t.GetCustomAttribute<FraudCaseAttribute>().FraudCase);
        }
    }
}

#pragma warning restore S3963 // "static" fields should be initialized inline