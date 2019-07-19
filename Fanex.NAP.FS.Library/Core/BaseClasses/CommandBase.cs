namespace Fanex.NAP.FS.Library.Core.BaseClasses
{
    using Fanex.Data.Repository;
    using System;

    public class CommandBase : NonQueryCommand
    {
        public virtual int ErrCode { get; set; }

        public override string GetSettingKey()
        {
            throw new NotImplementedException();
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}