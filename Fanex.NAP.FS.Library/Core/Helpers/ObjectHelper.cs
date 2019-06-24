namespace Fanex.NAP.FS.Library.Core.Helpers
{
    using System;
    using System.Reflection;

    public static class ObjectHelper
    {
        public static T Pick<T>(this T? target, T defaultValue) where T : struct => target.HasValue ? target.Value : defaultValue;

        public static string Pick(this string target, string defaultValue = "-") => string.IsNullOrWhiteSpace(target) ? defaultValue : target;

        public static T ConvertObject<T>(object source)
        {
            var target = Activator.CreateInstance<T>();

            var fields = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var field in fields)
            {
                var value = field.GetValue(source);
                if (field.CanWrite)
                {
                    field.SetValue(target, value);
                }
            }
            return target;
        }
    }
}