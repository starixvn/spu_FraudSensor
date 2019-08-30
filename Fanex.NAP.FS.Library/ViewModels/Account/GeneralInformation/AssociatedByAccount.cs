namespace Fanex.NAP.FS.Library.ViewModels.Account.GeneralInformation
{
    public class AssociatedByAccount
    {
        public int AccountId { get; set; }

        public int AssociatedAccount { get; set; }

        public int AssociatedDevice { get; set; }

        public int SharedDevices { get; set; }

        public int NonShareDevices { get; set; }

        public int AccountException { get; set; }
    }
}