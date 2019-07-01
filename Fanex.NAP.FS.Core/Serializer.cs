
namespace Fanex.NAP.FS.Core
{
    using System.IO;
    using System.Xml.Serialization;

    public static class Serializer
    {
        public static T Deserialize<T>(string input) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            
            using (StreamReader reader = new StreamReader(input))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }
    }
}