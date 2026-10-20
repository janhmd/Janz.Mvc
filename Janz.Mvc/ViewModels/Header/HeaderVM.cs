namespace Janz.Mvc.ViewModels.Header
{
    public class HeaderVM
    {
        public bool SignedIn { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int CartCount { get; set; }
    }
}
