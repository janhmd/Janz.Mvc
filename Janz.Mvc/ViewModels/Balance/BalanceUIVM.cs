namespace Janz.Mvc.ViewModels.Balance
{
    // api/balance: daxili hesabın balansı və hərəkətləri
    public class BalanceUIVM
    {
        public decimal Balance { get; set; }
        public List<BalanceTransactionUIVM> Transactions { get; set; } = new List<BalanceTransactionUIVM>();
    }
}
