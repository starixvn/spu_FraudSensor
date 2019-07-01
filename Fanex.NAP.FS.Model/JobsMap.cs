namespace Fanex.NAP.FS.Model
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "JobsMap")]
    public class JobsMap
    {
        [XmlElement(ElementName = "JobDataExecutingModel")]
        public List<JobDataExecutingModel> JobDataExecutingModel { get; set; }
    }
}