using Fanex.NAP.FS.Library.ViewModels;
using System;

namespace Fanex.NAP.FS.Library.DTO
{
    public class CustomerAssociation
    {
        public int FromCustId { get; set; }

        public int ToCustId { get; set; }

        public string ToOWUsername { get; set; }

        public string CurrencyName { get; set; }

        public int RoleId { get; set; }

        public string RoleLevel {
            get
            {
                try
                {
                    return Enum.GetName(typeof(RoleLevel), this.RoleId);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public string SubscriberName { get; set; }
    }
}