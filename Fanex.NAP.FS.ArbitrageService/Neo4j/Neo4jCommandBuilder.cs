using Fanex.Graph.Data;
using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.NAP.FS.ArbitrageService.Neo4j
{
    public class Neo4jCommandBuilder : ICommandBuilder
    {
        private IDictionary<string, string> spDict = new Dictionary<string, string>();

        public Neo4jCommandBuilder(string scriptDir, string extension = "scrypt")
        {
            spDict =
                Directory.GetFiles(Utilities.ResolvePath(scriptDir), $"*.{extension}", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .ToDictionary(f => f.Name.Replace($".{extension}", ""), f =>
                {
                    using (var stream = new StreamReader(f.FullName))
                    {
                        return stream.ReadToEnd();
                    }
                });
        }

        public Type DriverType { get; } = typeof(IDriver);

        public string BuildCommand(GraphSetting setting) => spDict[setting.SpName];
    }
}