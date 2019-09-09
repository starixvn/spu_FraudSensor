namespace Fanex.NAP.FS.Library.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class IEnumerableExtension
    {
        public static string Join<T, TR>(this IEnumerable<T> data, Func<T,TR> selector, string seperator = ",")
        {
            return string.Join(seperator, data.Select(selector));
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> data, Func<TSource, object> predicate)
        {            
            return from item in data.GroupBy(predicate) select item.First();
        }
    }
}
