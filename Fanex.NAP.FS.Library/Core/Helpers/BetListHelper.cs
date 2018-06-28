using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Fanex.NAP.FS.Library.Core.Helpers
{
    public static class BetListHelper
    {
        private static readonly Regex htmlReg = new Regex("<(.*?)>");

        public static string ExtractStringFromHtml(string htmlProp)
        {
            if (!string.IsNullOrEmpty(htmlProp))
            {
                return
                    htmlReg
                    .Matches(htmlProp)
                    .Cast<Match>()
                    .SelectMany(m => m.Captures.Cast<Capture>())
                    .Select(c => c.Value)
                    .Distinct()
                    .Aggregate(new StringBuilder(htmlProp), (str, tag) => str.Replace(tag, ""))
                    .Replace("&nbsp;", " ")
                    .ToString();
            }

            return htmlProp;
        }
    }
}