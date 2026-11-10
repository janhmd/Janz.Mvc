using Janz.Mvc.Helpers;
using Janz.Mvc.ViewModels.Journal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Controllers
{
    // Jurnal və məqalə səhifəsi (dizayn: Janz.Front/journal.html, article.html). Məqalələr API-dən gəlir: api/articles.
    // Mövzu, teq və sıralama brauzerdə işləyir (bütün dərc olunmuş məqalələr bir dəfəyə gəlir)
    public class JournalController : ApiControllerBase
    {
        public JournalController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? topic, string? tag, string? q)
        {
            var (articles, _) = await GetAsync<List<ArticleCardUIVM>>("api/articles");
            if (articles is null) ViewBag.Error = UnavailableMessage;

            // Başlıqdakı axtarış ("In journal"): başlıq, giriş, mövzu və teqlərdə
            var query = q?.Trim();
            if (articles is not null && !string.IsNullOrEmpty(query))
            {
                articles = articles.Where(a => a.Title.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                    || a.Lead.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                    || a.Topic.Contains(query, StringComparison.CurrentCultureIgnoreCase)
                    || a.Tags.Any(t => t.Contains(query, StringComparison.CurrentCultureIgnoreCase))).ToList();
            }

            return View(new JournalPageVM
            {
                Articles = articles ?? new List<ArticleCardUIVM>(),
                Topic = JournalPageVM.Topics.Contains(topic) ? topic : null,
                Tag = JournalPageVM.Tags.Contains(tag) ? tag : null,
                Query = query
            });
        }

        [HttpGet("/Journal/Article/{slug}")]
        public async Task<IActionResult> Article(string slug)
        {
            var seen = (Request.Cookies["janz_read"] ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            var firstVisit = !seen.Contains(slug);

            var (article, _) = await GetAsync<ArticleUIVM>($"api/articles/{Uri.EscapeDataString(slug)}?view={firstVisit.ToString().ToLower()}");
            if (article is null) return NotFound();

            if (firstVisit)
            {
                seen.Add(slug);
                var value = string.Join(",", seen.TakeLast(30));
                Response.Cookies.Append("janz_read", value, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddHours(12), HttpOnly = true, SameSite = SameSiteMode.Lax, IsEssential = true });
            }

            var (comments, _) = await GetAsync<List<ArticleCommentUIVM>>($"api/articles/{article.Id}/comments");

            return View(new ArticlePageVM
            {
                Article = article,
                Blocks = ArticleBody.Parse(article.Body, article.Images),
                Comments = comments ?? new List<ArticleCommentUIVM>()
            });
        }

        // Bəyənmə və saxlama düymələri (fetch): API təkrar basmanı nəzərə almır, say brauzerdə dəyişir
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(int id, bool on)
        {
            var path = $"api/articles/{id}/like";
            var (success, errors, status) = on ? await PostAsync(path, new { }) : await DeleteAsync(path);
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();
            if (!success) return BadRequest(new { errors });

            return Ok(new { on });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(int id, bool on)
        {
            var path = $"api/articles/{id}/save";
            var (success, errors, status) = on ? await PostAsync(path, new { }) : await DeleteAsync(path);
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();
            if (!success) return BadRequest(new { errors });

            return Ok(new { on });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(int id, string slug, string? text)
        {
            var (_, errors, status) = await PostAsync<ArticleCommentUIVM>($"api/articles/{id}/comments", new { text });
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (errors.Length > 0) TempData["CommentError"] = string.Join(" ", errors);

            return Redirect($"/Journal/Article/{Uri.EscapeDataString(slug)}#comments");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId, string slug)
        {
            var (success, errors, status) = await DeleteAsync($"api/articles/comments/{commentId}");
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (!success) TempData["CommentError"] = string.Join(" ", errors);

            return Redirect($"/Journal/Article/{Uri.EscapeDataString(slug)}#comments");
        }

        // Şərhə səs (fetch): 1, -1 və ya 0
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int commentId, int value)
        {
            var (comment, errors, status) = await PostAsync<ArticleCommentUIVM>($"api/articles/comments/{commentId}/vote", new { value });
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();
            if (comment is null) return BadRequest(new { errors });

            return Ok(new { likes = comment.Likes, dislikes = comment.Dislikes, myVote = comment.MyVote });
        }
    }
}
