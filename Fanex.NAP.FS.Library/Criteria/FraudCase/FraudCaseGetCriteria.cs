namespace Fanex.NAP.FS.Library.Criteria
{
    using Data.Repository;

    public class FraudCaseGetCriteria : CriteriaBase
    {
        public int? IsInternal { get; set; }

        public int? IsActive { get; set; }

        public int Environment { get; set; }

        public FraudCaseGetCriteria(int? isInternal, int? isActive, int environment = 1)
        {
            IsInternal = isInternal;
            IsActive = isActive;
            Environment = environment;
        }

        public override string GetSettingKey()
        {
            return "FS_FraudCase_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}