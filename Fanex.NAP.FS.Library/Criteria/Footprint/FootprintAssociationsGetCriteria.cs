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

        public override string GetSettingKey() => "DW_GetListAssociationByCustomer";

        public override bool IsValid() => true;
    }
}