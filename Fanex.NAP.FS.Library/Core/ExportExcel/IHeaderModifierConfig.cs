using NPOI.SS.UserModel;

namespace Fanex.NAP.FS.Library.Core.ExportExcel
{
    public interface IHeaderModifierConfig
    {
        void Modify(IRow row);
    }
}
