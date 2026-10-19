using Janz.Mvc.ViewModels.Orders;

namespace Janz.Mvc.Services
{
    public class OrderListCallResult
    {
        public bool Success { get; set; }
        public bool Unauthorized { get; set; }
        public List<OrderUIVM> Orders { get; set; } = new List<OrderUIVM>();
        public List<string> Errors { get; set; } = new List<string>();

        public static OrderListCallResult Ok(List<OrderUIVM> orders)
        {
            return new OrderListCallResult { Success = true, Orders = orders };
        }

        public static OrderListCallResult Fail(List<string> errors, bool unauthorized = false)
        {
            return new OrderListCallResult { Errors = errors, Unauthorized = unauthorized };
        }
    }
}
