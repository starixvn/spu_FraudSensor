namespace Fanex.NAP.FS.Library.Core
{
    public class Legend
    {
        public Legend(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
