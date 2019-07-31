namespace Fanex.NAP.FS.Library.Models.Account
{
    using Fanex.NAP.FS.Library.Models.Subscriber;

    public class Account
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string UserName2 { get; set; }

        public Subscriber Subscriber { get; set; }
    }
}