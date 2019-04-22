namespace Fanex.NAP.FS.Service
{
    using System.Collections.Generic;

    interface IExecutorService
    {        
        IEnumerable<T> PreCondition<T>();

        void Rule();
    }
}
