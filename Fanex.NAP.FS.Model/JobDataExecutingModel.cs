namespace Fanex.NAP.FS.Model
{
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "JobDataExecutingModel")]
    public class JobDataExecutingModel
    {
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Path")]
        public string Path { get; set; }
    }
}