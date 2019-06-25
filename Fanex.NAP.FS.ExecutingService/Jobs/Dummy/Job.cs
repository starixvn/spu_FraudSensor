using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.ExecutingService.Jobs.Dummy
{
    public class DummyJob : IJob
    {
        private static readonly Random Random = new Random();

        public async Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("DummyJob > " + DateTime.Now);

            await Task.Delay(TimeSpan.FromSeconds(Random.Next(1, 20)));

            if (Random.Next(2) == 0)
                throw new Exception("Fatal error example!");
        }
    }

}
