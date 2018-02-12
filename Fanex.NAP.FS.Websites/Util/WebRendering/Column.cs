namespace Fanex.NAP.FS.Websites.Util.WebRendering
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Linq;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public static string GetColumnJson<T>()
        {
            var t = typeof(T);

            var properties =
                    t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(prop => prop.GetCustomAttribute<IgnoreAttribute>() == null)
                    .ToDictionary(prop => prop.Name, GetColumnName);

            var json = JObject.FromObject(properties);

            return json.ToString();
        }

        private static string GetColumnName(PropertyInfo prop)
        {
            var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();

            if (columnAttr == null)
            {
                return prop.Name;
            }

            return columnAttr.Name;
        }
    }
}