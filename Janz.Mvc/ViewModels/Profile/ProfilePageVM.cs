using Janz.Mvc.ViewModels.Addresses;
using Janz.Mvc.ViewModels.Orders;

namespace Janz.Mvc.ViewModels.Profile
{
    public class ProfilePageVM
    {
        public List<AddressUIVM> Addresses { get; set; } = new List<AddressUIVM>();
        public List<OrderUIVM> Orders { get; set; } = new List<OrderUIVM>();
        public int? EditId { get; set; }
    }
}
