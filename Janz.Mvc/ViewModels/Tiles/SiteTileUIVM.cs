namespace Janz.Mvc.ViewModels.Tiles
{
    // api/tiles cavabındakı kart (Home və kataloq səhifələri)
    public class SiteTileUIVM
    {
        public string Title { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string? VideoPath { get; set; }
    }
}
