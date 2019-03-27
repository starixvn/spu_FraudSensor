using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Model
{
    public enum Action
    {
        None,       
        Get,
        Post,
        HealthCheck,
        Reply,
        Deactivating,
        Activating,
        Updating,
        Retry
    }
}
