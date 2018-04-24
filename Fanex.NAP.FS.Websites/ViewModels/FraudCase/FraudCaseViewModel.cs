namespace Fanex.NAP.FS.Websites.ViewModels.FraudCase
{
    using System.Collections.Generic;

    public class FraudCaseViewModel
    {
        public Library.Models.FraudCase.FraudCase FraudCaseInfo { get; set; }

        public IEnumerable<Library.DTO.FraudCase> FraudCaseList { get; set; }
    }
}