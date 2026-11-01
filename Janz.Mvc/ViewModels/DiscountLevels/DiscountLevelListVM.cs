namespace Janz.Mvc.ViewModels.DiscountLevels
{
    // Admin: bütün cədvəl bir formadır, birdən saxlanılır
    public class DiscountLevelListVM
    {
        public List<DiscountLevelVM> Levels { get; set; } = new List<DiscountLevelVM>();
    }
}
