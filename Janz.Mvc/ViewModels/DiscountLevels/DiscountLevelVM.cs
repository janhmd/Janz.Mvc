namespace Janz.Mvc.ViewModels.DiscountLevels
{
    // Admin: cədvəldə bir səviyyə (Levels[0].Name ...). Id boşdursa yeni səviyyədir
    public class DiscountLevelVM
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public decimal MinSpent { get; set; }
        public int Percent { get; set; }
        public string? Perks { get; set; }
        public int CustomerCount { get; set; }
    }
}
