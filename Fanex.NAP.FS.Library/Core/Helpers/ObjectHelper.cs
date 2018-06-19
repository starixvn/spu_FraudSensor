namespace Fanex.NAP.FS.Library.Core.Helpers
{
    public static class ObjectHelper
    {
        public static T Pick<T>(this T? target, T defaultValue) where T : struct => target.HasValue ? target.Value : defaultValue;
    }
}