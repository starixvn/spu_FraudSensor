using NPOI.SS.UserModel;

namespace Fanex.NAP.FS.Library.Core.ExportExcel
{
    public interface IDataFillerConfig<in T>
    {
        void Fill(IRow row, T obj);
    }
}
