namespace Janz.Mvc.ViewModels.Journal
{
    public class JournalPageVM
    {
        public static readonly string[] Topics = { "Brewing guides", "Brewing", "Industry", "Growing", "Roasting", "Marketing", "Tea" };
        public static readonly string[] Tags = { "Growing", "Varieties", "Roasting", "Brewing", "Industry", "Marketing", "Espresso" };

        public List<ArticleCardUIVM> Articles { get; set; } = new List<ArticleCardUIVM>();

        // Ünvandan gələn filtr (məqalədəki teqə basanda): JS səhifə açılanda onu seçir
        public string? Topic { get; set; }
        public string? Tag { get; set; }

        // Başlıqdakı axtarış sözü ("In journal")
        public string? Query { get; set; }
    }
}
