#pragma warning disable S1226 //// Method parameters and caught exceptions should not be reassigned

namespace Fanex.NAP.FS.Library.Core.Utilities
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Extentions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            var q = new Queue<T>();

            foreach (var item in source)
            {
                // check if q is full
                if(q.Count == chunksize)
                {
                    var ret = q;
                    q = new Queue<T>();
                    
                    yield return ret;
                }

                // enqueue item
                q.Enqueue(item);
            }

            yield return q;
        }
    }
}

#pragma warning restore S1226 //// Method parameters and caught exceptions should not be reassigned