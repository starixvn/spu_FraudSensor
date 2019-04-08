using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Service
{
    interface IFraudcaseService<T>
    {
        IEnumerable<T> PreCondition();

        void Rule(string matchIds, string associations);

        void Rule(string matchIds);
    }
}
