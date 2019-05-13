namespace Fanex.NAP.FS.Library.Models.FraudCaseTask
{
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    
    [XmlType(TypeName = "Association")]
    public class ArbitrageCustomer
    {
        [XmlAttribute]
        public int CustId { get; set; }

        [XmlAttribute]
        public int GroupId { get; set; }
    }
}