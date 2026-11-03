using System.Text.RegularExpressions;
using Janz.Mvc.ViewModels.Journal;

namespace Janz.Mvc.Helpers
{
    // Məqalənin mətnini hissələrə bölür: boş sətir yeni abzasdır, "## " başlıqdır,
    // "[photo 2: caption]" məqalənin 2-ci şəklini qoyur (caption istəyə bağlıdır)
    public static class ArticleBody
    {
        private static readonly Regex Photo = new(@"^\[photo\s+(\d+)\s*(?::\s*(.*))?\]$", RegexOptions.IgnoreCase);

        public static List<ArticleBlockVM> Parse(string body, List<string> images)
        {
            var blocks = new List<ArticleBlockVM>();
            ArticleBlockVM? text = null;
            var paragraph = new List<string>();

            void EndParagraph()
            {
                if (paragraph.Count == 0) return;
                text ??= new ArticleBlockVM { Kind = "Text" };
                text.Paragraphs.Add(string.Join(" ", paragraph));
                paragraph.Clear();
            }

            void EndText()
            {
                EndParagraph();
                if (text is not null) blocks.Add(text);
                text = null;
            }

            foreach (var raw in body.Replace("\r", string.Empty).Split('\n'))
            {
                var line = raw.Trim();
                if (line.Length == 0)
                {
                    EndParagraph();
                    continue;
                }

                if (line.StartsWith("## "))
                {
                    EndText();
                    blocks.Add(new ArticleBlockVM { Kind = "Heading", Heading = line[3..].Trim() });
                    continue;
                }

                var photo = Photo.Match(line);
                if (photo.Success)
                {
                    EndText();
                    var index = int.Parse(photo.Groups[1].Value) - 1;
                    if (index >= 0 && index < images.Count)
                    {
                        var caption = photo.Groups[2].Value.Trim();
                        blocks.Add(new ArticleBlockVM { Kind = "Photo", Image = images[index], Caption = caption.Length == 0 ? null : caption });
                    }
                    continue;
                }

                paragraph.Add(line);
            }

            EndText();
            return blocks;
        }

        // 2140 -> 2K, 912 -> 912
        public static string Short(int value)
        {
            return value >= 1000 ? (value / 1000) + "K" : value.ToString();
        }
    }
}
