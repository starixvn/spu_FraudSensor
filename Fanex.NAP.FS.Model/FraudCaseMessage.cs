namespace Fanex.NAP.FS.Model
{
    using System;

    [Serializable]
    public class FraudCaseMessage
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Action Action { get; set; }

        public string Message { get; set; }

        public string ServiceName { get; set; }

        public bool Status { get; set; }
    }
}
