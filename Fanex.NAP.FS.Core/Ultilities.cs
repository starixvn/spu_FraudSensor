namespace Fanex.NAP.FS.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    public static class Ultilities
    {
        public static void WriteLog(string filename, IEnumerable<string> body)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            File.WriteAllLines(filePath, body);
        }

        public static void WriteLog(string filename, string body)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            File.AppendAllText(filePath, body);
        }

        public static string XMLConvert(object data)
        {
            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var settings = new XmlWriterSettings {Indent = true, OmitXmlDeclaration = true};

            var serializer = new XmlSerializer(data.GetType(),new XmlRootAttribute("Root"));

            using (var stringWriter = new System.IO.StringWriter())
            {
                using (var writer = XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(writer, data, emptyNamespaces);

                    return stringWriter.ToString();
                }
            }
        }
    }
}
