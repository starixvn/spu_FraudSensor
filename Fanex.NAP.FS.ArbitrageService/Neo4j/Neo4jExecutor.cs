using Fanex.Graph.Data;
using Neo4j.Driver.V1;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.FS.ArbitrageService.Neo4j
{
    public class Neo4jExecutor : ICommandExecutor<IDriver>
    {
        public ExecutionResult<IEnumerable<T>> Execute<T>(IDriver driver, string command, object parameters = null)
        {
            var statementResult = RunStatement(driver, command, parameters);

            var counters = statementResult.Summary.Counters;
            
            return new ExecutionResult<IEnumerable<T>>
            {
                AffectedNodes = counters.NodesCreated + counters.NodesDeleted,
                AffectedRelationship = counters.RelationshipsCreated + counters.RelationshipsDeleted,
                Data = statementResult.Select(ParseRecord<T>).ToArray()
            };
        }

        public ExecutionResult Execute(IDriver driver, string command, object parameters = null)
        {
            var statementResult = RunStatement(driver, command, parameters);

            var counters = statementResult.Summary.Counters;

            return new ExecutionResult
            {
                AffectedNodes = counters.NodesCreated + counters.NodesDeleted,
                AffectedRelationship = counters.RelationshipsCreated + counters.RelationshipsDeleted,
            };
        }

        public ExecutionResult<T> ExecuteSingle<T>(IDriver driver, string command, object parameters = null)
        {
            var statementResult = RunStatement(driver, command, parameters);

            var counters = statementResult.Summary.Counters;

            return new ExecutionResult<T>
            {
                AffectedNodes = counters.NodesCreated + counters.NodesDeleted,
                AffectedRelationship = counters.RelationshipsCreated + counters.RelationshipsDeleted,
                Data = ParseRecord<T>(statementResult.First())
            };
        }

        private IStatementResult RunStatement(IDriver driver, string command, object parameters)
        {
            IStatementResult result = null;
            using (var session = driver.Session())
            {
                result = session.WriteTransaction(tx => tx.Run(command, parameters));
            }

            return result;
        }

        private T ParseRecord<T>(IRecord record)
        {
            var t = typeof(T);

            if (t.IsPrimitive || t == typeof(string) || t == typeof(decimal))
            {
                return record[0].As<T>();
            }
            else if (t == typeof(DateTime))
            {
                object obj = DateTime.Parse(record[0].As<string>());

                return (T)obj;
            }

            return JObject.FromObject(record.Values).ToObject<T>();
        }
    }
}