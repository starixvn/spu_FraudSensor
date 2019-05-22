using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Fanex.NAP.FS.Core
{
    public class XmlConvertor
    {
        public static string XMLConvert(object data)
        {
            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };

            var serializer = new XmlSerializer(data.GetType(), new XmlRootAttribute("Root"));

            using (var stringWriter = new System.IO.StringWriter())
            {
                using (var writer = XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(writer, data, emptyNamespaces);

                    return stringWriter.ToString();
                }
            }
        }

        public string XmlConvertByIsolatedStorage(object data, string xmlFileName)
        {
            string result;

            using (var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                using (var isoStream = new IsolatedStorageFileStream(xmlFileName, FileMode.Create, isoStore))
                {
                    var serializer = new XmlSerializer(data.GetType(), new XmlRootAttribute("Root"));
                    var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
                    var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

                    using (var writer = XmlWriter.Create(isoStream, settings))
                    {
                        serializer.Serialize(writer, data, emptyNamespaces);

                        writer.Flush();
                        writer.Close();
                    }

                    isoStream.Close();
                }
                using (var streamData = isoStore.OpenFile(xmlFileName, FileMode.Open))
                {
                    using (var reader = new StreamReader(streamData))
                    {
                        result = reader.ReadToEnd();
                        reader.Close();
                    }

                    streamData.Close();
                    isoStore.DeleteFile(xmlFileName);
                }
            }

            return result;
        }
    }
}
