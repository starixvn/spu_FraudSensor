namespace Fanex.NAP.FS.Library.Models.FraudCaseTask
{
    using Fanex.NAP.FS.Library.Core.Helpers.Xml;

    [XName("Association")]
    public class ArbitrageCustomer
    {
        public int CustId { get; set; }

        public int GroupId { get; set; }
    }
}