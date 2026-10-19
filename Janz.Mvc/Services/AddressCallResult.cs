using Janz.Mvc.ViewModels.Account;

namespace Janz.Mvc.Services
{
    public class AddressCallResult
    {
        public bool Success { get; set; }
        public bool Unauthorized { get; set; }
        public AddressUIVM Address { get; set; } = new AddressUIVM();
        public List<AddressUIVM> Addresses { get; set; } = new List<AddressUIVM>();
        public List<string> Errors { get; set; } = new List<string>();

        public static AddressCallResult Ok(List<AddressUIVM> addresses)
        {
            return new AddressCallResult { Success = true, Addresses = addresses };
        }

        public static AddressCallResult Ok(AddressUIVM address)
        {
            return new AddressCallResult { Success = true, Address = address };
        }

        public static AddressCallResult Ok()
        {
            return new AddressCallResult { Success = true };
        }

        public static AddressCallResult Fail(List<string> errors, bool unauthorized = false)
        {
            return new AddressCallResult { Errors = errors, Unauthorized = unauthorized };
        }
    }
}
