using Janz.Mvc.ViewModels.Orders;

namespace Janz.Mvc.Services
{
    public class OrderCallResult
    {
        public bool Success { get; set; }
        public bool Unauthorized { get; set; }
        public OrderUIVM Order { get; set; } = new OrderUIVM();
        public List<string> Errors { get; set; } = new List<string>();

        public static OrderCallResult Ok(OrderUIVM order)
        {
            return new OrderCallResult { Success = true, Order = order };
        }

        public static OrderCallResult Fail(List<string> errors, bool unauthorized = false)
        {
            return new OrderCallResult { Errors = errors, Unauthorized = unauthorized };
        }
    }
}
