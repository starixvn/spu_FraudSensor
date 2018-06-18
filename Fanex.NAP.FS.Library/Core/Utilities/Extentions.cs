#pragma warning disable S1226 //// Method parameters and caught exceptions should not be reassigned

namespace Fanex.NAP.FS.Library.Core.Utilities
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Extentions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);

                source = source.Skip(chunksize);
            }
        }
    }
}

#pragma warning restore S1226 //// Method parameters and caught exceptions should not be reassigned