namespace Fanex.NAP.FS.Library.ViewModels.Account.Associated
{
    using System;

    public class AssociatedDeviceByAccount
    {
        public string DeviceNo { get; set; }

        public DateTime FirstUsed { get; set; }

        public DateTime LastUsed { get; set; }

        public DateTime AssociatedDate { get; set; }

        public int TotalUses { get; set; }

        public string GraphURL { get; set; }
    }
}