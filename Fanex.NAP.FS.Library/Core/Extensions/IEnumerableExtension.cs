using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Library.Core.Extensions
{
    public static class IEnumerableExtension
    {
        public static string Join<T, TR>(this IEnumerable<T> data, Func<T,TR> selector, string seperator = ",")
        {
            return string.Join(seperator, data.Select(selector));
        }
    }
}
