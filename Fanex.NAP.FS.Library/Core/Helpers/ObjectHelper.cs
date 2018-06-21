namespace Fanex.NAP.FS.Library.Core.Helpers
{
    public static class ObjectHelper
    {
        public static T Pick<T>(this T? target, T defaultValue) where T : struct => target.HasValue ? target.Value : defaultValue;

        public static string Pick(this string target, string defaultValue = "-") => string.IsNullOrWhiteSpace(target) ? defaultValue : target;
    }
}