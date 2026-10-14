using Janz.Mvc.ViewModels.Cart;

namespace Janz.Mvc.Services
{
    public class CartCallResult
    {
        public bool Success { get; set; }
        public bool Unauthorized { get; set; }
        public CartUIVM Cart { get; set; } = new CartUIVM();
        public List<string> Errors { get; set; } = new List<string>();

        public static CartCallResult Ok(CartUIVM cart)
        {
            return new CartCallResult { Success = true, Cart = cart };
        }

        public static CartCallResult Fail(List<string> errors, bool unauthorized = false)
        {
            return new CartCallResult { Errors = errors, Unauthorized = unauthorized };
        }
    }
}
