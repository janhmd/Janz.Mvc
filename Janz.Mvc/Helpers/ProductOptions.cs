using Microsoft.AspNetCore.Html;

namespace Janz.Mvc.Helpers
{
    // API-dəki Domain/Constants/BrewMethods və GrindOptions ilə eyni olmalıdır
    public static class ProductOptions
    {
        public static readonly string[] BrewMethods =
        {
            "Espresso machine", "Turkish coffee", "Moka pot", "Pour over", "French press", "AeroPress", "Drip coffee maker", "Cup", "Capsule machine"
        };

        public const string WholeBeans = "Whole beans";

        public static readonly string[] Grinds =
        {
            WholeBeans,
            "For French press (coarse)",
            "For drip coffee maker (coarse)",
            "For Chemex (coarse)",
            "For pour-over (medium)",
            "For AeroPress (medium)",
            "For moka pot (medium)",
            "For espresso, coarse (medium)",
            "For espresso, fine (fine)",
            "For Turkish coffee (fine)",
            "For a cup (medium)"
        };

        // Admin formasında paket növü üçün təkliflər (başqasını da yazmaq olar)
        public static readonly string[] Formats = { "Pack", "Nespresso capsules 10 pcs", "Nespresso capsules 40 pcs", "Drip bags" };

        // Məhsul səhifəsində hazırlanma üsullarının ikonları (SVG path)
        private static readonly Dictionary<string, string> BrewIcons = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Espresso machine"] = "M5 8h10v8H5zM15 10h3a2 2 0 0 1 0 4h-3",
            ["Turkish coffee"] = "M8 9h6l-1 8H9zM9 6h4",
            ["Moka pot"] = "M7 10h10v6a3 3 0 0 1-3 3H10a3 3 0 0 1-3-3zM9 7h6",
            ["Pour over"] = "M5 6h14l-5 7h-4zM10 13v3h4v-3M7 19h10",
            ["French press"] = "M7 6h10v13H7zM12 3v9M9 12h6M17 9h2v6h-2",
            ["AeroPress"] = "M8 4h8v12H8zM6 16h12M9 19h6M10 8h4",
            ["Drip coffee maker"] = "M6 4h12v4H6zM8 8l2 4h4l2-4M8 14h8v6H8z",
            ["Cup"] = "M5 9h11v5a4 4 0 0 1-4 4H9a4 4 0 0 1-4-4zM16 10h2a2 2 0 0 1 0 4h-2M8 5v2M12 5v2",
            ["Capsule machine"] = "M6 4h12v6H6zM9 10v3h6v-3M7 20h10M9 16h6v4H9z"
        };

        public static string BrewIcon(string method) => BrewIcons.TryGetValue(method, out var icon) ? icon : BrewIcons["Cup"];

        // API-dəki Domain/Constants/TasteGroups ilə eyni olmalıdır (kataloqdakı Taste filtri)
        public static readonly string[] TasteGroups = { "Chocolate / Nuts", "Flowers / Citrus", "Fruits / Berries", "Alcohol / Funky" };

        // Kartın yuxarısındakı təyinat üçün təkliflər (başqasını da yazmaq olar)
        public static readonly string[] Uses = { "For espresso", "For filter", "For espresso and filter", "For milk drinks", "For Turkish coffee" };

        // Kartda notun rəngi (site.css-dəki note-* sinifləri). Tanınmayan not rəngsiz qalır
        private static readonly (string Word, string Css)[] NoteColors =
        {
            ("dark chocolate", "dark-chocolate"), ("chocolate", "chocolate"), ("cocoa", "cocoa"),
            ("dark caramel", "dark-caramel"), ("candied", "candied"), ("peanut", "peanut"), ("nut", "nuts"), ("almond", "nuts"), ("hazelnut", "nuts"), ("biscuit", "biscuit"), ("cookie", "biscuit"),
            ("nougat", "nougat"), ("caramel", "caramel"), ("honey", "honey"), ("spice", "spices"), ("cinnamon", "spices"),
            ("strawberr", "strawberry"), ("raspberr", "raspberry"), ("cherr", "cherry"), ("berr", "berries"),
            ("flower", "flowers"), ("jasmine", "flowers"), ("tangerine", "tangerine"), ("orange", "orange"),
            ("lemon", "lemon"), ("citrus", "citrus"), ("mango", "mango"), ("apricot", "apricot"), ("peach", "peach"),
            ("pineapple", "pineapple"), ("apple", "apple"), ("grape", "grapes"), ("passion", "passion"), ("vermouth", "vermouth"), ("wine", "vermouth"), ("fruit", "fruit")
        };

        public static string? NoteClass(string note)
        {
            var match = NoteColors.FirstOrDefault(m => note.Contains(m.Word, StringComparison.OrdinalIgnoreCase));
            return match.Css is null ? null : "note-" + match.Css;
        }

        // Kartdakı və məhsul səhifəsindəki cümlə: "Dense coffee with notes of <nuts>, <chocolate> and <caramel>".
        // Bir sətirdə qurulur ki, notlarla vergüllər arasında boşluq qalmasın (mətnlər HTML-encode olunur)
        public static IHtmlContent TasteLine(string? taste, List<string> notes, string? fallback = null)
        {
            var line = new HtmlContentBuilder();
            if (notes.Count == 0) return line.Append(taste ?? fallback ?? string.Empty);

            line.Append(taste is null ? "Notes of " : taste + " with notes of ");
            for (var i = 0; i < notes.Count; i++)
            {
                if (i > 0) line.Append(i == notes.Count - 1 ? " and " : ", ");
                var css = NoteClass(notes[i]);
                line.AppendHtml("<span class=\"note" + (css is null ? string.Empty : " " + css) + "\">");
                line.Append(notes[i]);
                line.AppendHtml("</span>");
            }

            return line;
        }

        // Üyütmə seçimi yalnız paketdəki qəhvədə var
        public static bool HasGrind(string? format) => string.Equals(format, "Pack", StringComparison.OrdinalIgnoreCase);
    }
}
