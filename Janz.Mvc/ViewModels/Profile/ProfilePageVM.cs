using Janz.Mvc.ViewModels.Addresses;
using Janz.Mvc.ViewModels.Orders;

namespace Janz.Mvc.ViewModels.Profile
{
    public class ProfilePageVM
    {
        public List<AddressUIVM> Addresses { get; set; } = new List<AddressUIVM>();
        public List<OrderUIVM> Orders { get; set; } = new List<OrderUIVM>();
        public int? EditId { get; set; }
        public Janz.Mvc.ViewModels.Balance.BalanceUIVM Balance { get; set; } = new Janz.Mvc.ViewModels.Balance.BalanceUIVM();
        // Kabinetdə "Reading in the journal" -> "Bookmarks"
        public List<Janz.Mvc.ViewModels.Journal.ArticleCardUIVM> SavedArticles { get; set; } = new List<Janz.Mvc.ViewModels.Journal.ArticleCardUIVM>();
        public ProfileUIVM Profile { get; set; } = new ProfileUIVM();
        public StatisticsUIVM Statistics { get; set; } = new StatisticsUIVM();
    }
}
