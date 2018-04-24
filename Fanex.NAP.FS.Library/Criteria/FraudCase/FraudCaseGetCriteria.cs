namespace Fanex.NAP.FS.Library.Criteria
{
    using Data.Repository;

    public class FraudCaseGetCriteria : CriteriaBase
    {
        public bool IsInternal { get; set; }

        public bool IsActive { get; set; }

        public int Environment { get; set; }

        public FraudCaseGetCriteria(bool isInternal = true, bool isActive = true, int environment = 1)
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