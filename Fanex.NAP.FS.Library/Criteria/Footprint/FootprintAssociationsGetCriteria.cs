namespace Fanex.NAP.FS.Library.Criteria.Footprint
{
    using Fanex.Data.Repository;

    public class FootprintAssociationsGetCriteria : CriteriaBase
    {
        public FootprintAssociationsGetCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; set; }

        public override string GetSettingKey() => "FS_Footprint_GetAssociationsByCustId";

        public override bool IsValid() => true;
    }
}