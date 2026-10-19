using Janz.Mvc.ViewModels.Orders;

namespace Janz.Mvc.ViewModels.Account
{
    public class AccountPageVM
    {
        public List<AddressUIVM> Addresses { get; set; } = new List<AddressUIVM>();
        public List<OrderUIVM> Orders { get; set; } = new List<OrderUIVM>();
        public int? EditId { get; set; }
    }
}
