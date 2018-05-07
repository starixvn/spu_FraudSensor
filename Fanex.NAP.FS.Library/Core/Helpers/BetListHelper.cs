using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Library.Core.Helpers
{
    public static class BetListHelper
    {
        private static readonly Regex regex = new Regex(">(.*?)<");

        public static string ExtractStringFromHtml(string htmlProp)
        {
            var match = regex.Match(htmlProp);

            if (match.Success)
            {
                try
                {
                    var str = match.Captures.Cast<Capture>().First(c => !string.IsNullOrEmpty(c.Value)).Value;

                    var builder = new StringBuilder(str);

                    return builder.Remove(0, 1).Remove(builder.Length - 1, 1).Replace("&nbsp;", "").ToString();
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(htmlProp, ex);
                }
            }

            return null;
        }
    }
}
