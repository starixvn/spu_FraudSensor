namespace Fanex.NAP.FS.Library.Criteria.MonitorFuncAccess
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class StatsAccessmentInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_SystemId")]
        public long SystemId { get; set; }

        [SpParam(SqlParamName = "ip_FunctionId")]
        public int FunctionId { get; set; }

        [SpParam(SqlParamName = "ip_UserId")]
        public int UserId { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "SPU_Stats_Accessment_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
