using System;

namespace Fanex.NAP.FS.Websites.ViewModels.FraudCase.DelayedGoal
{
    public class Customer
    {
        public Customer(Models.FraudCase.DelayedGoal.Customer customer)
        {
            Id = customer.CustId;
            Name = customer.AccountName ?? "-";
            Creator = customer.CreatedBy ?? "-";
            AddedDate = customer.CreatedDate.ToString("MMM dd,yyyy hh:mm tt");
        }

        public int Id { get; set; }

        public string Name  { get; set; }

        public string Creator { get; set; }

        public string AddedDate { get; set; }
    }
}