using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Service
{
    interface IExecutorService
    {
        IEnumerable<T> PreCondition<T>();

        void Rule(string matchIds, string param1);

        void Rule(string matchIds);

        void Rule();
    }
}
