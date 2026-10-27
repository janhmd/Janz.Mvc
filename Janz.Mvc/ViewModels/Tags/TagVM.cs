namespace Janz.Mvc.ViewModels.Tags
{
    // Admin panel: teq (api/admin/tags)
    public class TagVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}
