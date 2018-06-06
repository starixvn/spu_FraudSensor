namespace Fanex.NAP.FS.Library.Core.BaseClasses
{
    using Fanex.NAP.FS.Library.Models;
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class FraudCaseAttribute : Attribute
    {
        public FraudCaseAttribute(FraudCaseId fraudCase)
        {
            FraudCase = fraudCase;
        }

        public FraudCaseId FraudCase { get; }
    }
}