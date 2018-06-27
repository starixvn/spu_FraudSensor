using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fanex.NAP.FS.Library.Core.Helpers
{
    public static class BetListHelper
    {
        private static readonly Regex regex = new Regex(">(.*?)<");

        public static string ExtractStringFromHtml(string htmlProp)
        {
            if (!string.IsNullOrEmpty(htmlProp))
            {
                var match = regex.Match(htmlProp);

                if (match.Success)
                {
                    try
                    {
                        return match.Captures.Cast<Capture>()
                            .Select(c => c.Value.Remove(c.Length - 1, 1).Remove(0, 1))
                            .Join(" ")
                            .Replace("&nbsp;", "");
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(htmlProp, ex);
                    }
                }

                return htmlProp.Replace("&nbsp;", ""); 
            }

            return htmlProp;
        }
    }
}