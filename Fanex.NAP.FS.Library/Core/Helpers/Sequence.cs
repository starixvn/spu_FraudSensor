namespace Fanex.NAP.FS.Library.Core.Helpers
{
    using System.Collections.Generic;

    public static class Sequence
    {
        public static string Join<T>(this IEnumerable<T> data, string separator = ",") => string.Join(separator, data);
    }
}