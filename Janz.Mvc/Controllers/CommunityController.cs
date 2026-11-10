using Janz.Mvc.ViewModels.Community;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Janz.Mvc.Controllers
{
    // Community lenti və açıq profillər (dizayn: Janz.Front/community.html, profile.html). Məlumat API-dən gəlir: api/posts, api/community
    public class CommunityController : ApiControllerBase
    {
        private static readonly string[] Tags = { "Pour over", "Espresso", "Filter", "cezve", "French press", "AeroPress", "Drip", "Chemex", "Cold brew", "Light roast", "Dark roast" };

        public CommunityController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? tab, string? tag, string? search, string? scope)
        {
            var query = $"tab={Uri.EscapeDataString(tab ?? "best")}&tag={Uri.EscapeDataString(tag ?? string.Empty)}&search={Uri.EscapeDataString(search ?? string.Empty)}&scope={Uri.EscapeDataString(scope ?? "everywhere")}";
            var (posts, _) = await GetAsync<List<PostUIVM>>($"api/posts?{query}");

            return View(new CommunityPageVM
            {
                Posts = posts ?? new List<PostUIVM>(),
                Tab = tab == "new" ? "new" : "best",
                Tag = tag,
                Search = search,
                Scope = scope ?? "everywhere",
                Tags = Tags.ToList()
            });
        }

        // İstifadəçinin açıq profili: id Nickname və ya istifadəçi id-sidir
        [HttpGet]
        public async Task<IActionResult> Profile(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var (profile, status) = await GetAsync<CommunityProfileUIVM>($"api/community/profiles/{Uri.EscapeDataString(id)}");
            if (profile is null) return status == HttpStatusCode.NotFound ? NotFound() : RedirectToAction(nameof(Index));

            return View(profile);
        }

        // Post və şərh üçün səs (fetch): 1, -1 və ya 0
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(string kind, int id, int value)
        {
            var path = kind == "comment" ? $"api/posts/comments/{id}/vote" : $"api/posts/{id}/vote";
            var (result, errors, status) = await PostAsync<VoteResultUIVM>(path, new { value });
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();
            if (result is null) return BadRequest(new { errors });

            return Ok(new { score = result.Score, myVote = result.MyVote });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(int id, bool on)
        {
            var path = $"api/posts/{id}/save";
            var (success, errors, status) = on ? await PostAsync(path, new { }) : await DeleteAsync(path);
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();
            if (!success) return BadRequest(new { errors });

            return Ok(new { on });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(string userId, bool on)
        {
            var path = $"api/community/follow/{Uri.EscapeDataString(userId)}";
            var (success, errors, status) = on ? await PostAsync(path, new { }) : await DeleteAsync(path);
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();
            if (!success) return BadRequest(new { errors });

            return Ok(new { on });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block(string userId, bool on)
        {
            var path = $"api/community/block/{Uri.EscapeDataString(userId)}";
            var (success, errors, status) = on ? await PostAsync(path, new { }) : await DeleteAsync(path);
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();
            if (!success) return BadRequest(new { errors });

            return Ok(new { on });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(string kind, int id, string? reason)
        {
            var path = kind == "comment" ? $"api/posts/comments/{id}/report" : $"api/posts/{id}/report";
            var (success, errors, status) = await PostAsync(path, new { reason });
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();
            if (!success) return BadRequest(new { errors });

            return Ok(new { sent = true });
        }

        // Şərh və cavab (fetch, şəkil ola bilər): yeni şərhin hazır HTML-i qayıdır
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(int postId, int? parentId, string? text, IFormFile? image)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(text ?? string.Empty), "Text");
            if (parentId.HasValue) form.Add(new StringContent(parentId.Value.ToString()), "ParentId");
            if (image is not null && image.Length > 0)
            {
                var content = new StreamContent(image.OpenReadStream());
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
                form.Add(content, "Image", image.FileName);
            }

            var (comment, errors, status) = await PostFormAsync<PostCommentUIVM>($"api/posts/{postId}/comments", form);
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();
            if (comment is null) return BadRequest(new { errors });

            return PartialView("_FeedNote", comment);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var (success, errors, status) = await DeleteAsync($"api/posts/comments/{id}");
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();
            if (!success) return BadRequest(new { errors });

            return Ok(new { deleted = true });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var (success, errors, status) = await DeleteAsync($"api/posts/{id}");
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();
            if (!success) return BadRequest(new { errors });

            return Ok(new { deleted = true });
        }

        // Kabinetdəki "Community profile" formasından yeni post (şəkillər ola bilər)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(string? text, int? productId, int? rating, string? brew, string? recipe, string? dose, string? water, string? grind, string? time, string? tags, List<IFormFile>? images)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(text ?? string.Empty), "Text");
            if (productId.HasValue) form.Add(new StringContent(productId.Value.ToString()), "ProductId");
            if (productId.HasValue && rating.HasValue) form.Add(new StringContent(rating.Value.ToString()), "Rating");
            foreach (var (name, value) in new[] { ("Brew", brew), ("Recipe", recipe), ("Dose", dose), ("Water", water), ("Grind", grind), ("Time", time) })
            {
                if (!string.IsNullOrWhiteSpace(value)) form.Add(new StringContent(value), name);
            }

            foreach (var tag in (tags ?? string.Empty).Split('|', StringSplitOptions.RemoveEmptyEntries)) form.Add(new StringContent(tag), "Tags");
            foreach (var image in images ?? new List<IFormFile>())
            {
                if (image.Length == 0) continue;
                var content = new StreamContent(image.OpenReadStream());
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
                form.Add(content, "Images", image.FileName);
            }

            var (post, errors, status) = await PostFormAsync<PostUIVM>("api/posts", form);
            if (status == HttpStatusCode.Unauthorized) return await SessionExpiredAsync();
            if (post is null)
            {
                TempData["CommunityError"] = string.Join(" ", errors);
                return Redirect("/Profile#community");
            }

            return Redirect("/Community?tab=new");
        }

        // Bio və "How I cook" (fetch)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string? bio, string[]? cookMethods)
        {
            var (success, errors, status) = await PutAsync("api/community/me", new { bio, cookMethods = cookMethods ?? Array.Empty<string>() });
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();
            if (!success) return BadRequest(new { errors });

            return Ok(new { saved = true });
        }

        // Abunəçilər və abunəliklər siyahısı (kabinetdəki pəncərə üçün hazır HTML)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> People(string kind)
        {
            var userId = await CurrentUserIdAsync();
            var path = kind == "subscriptions" ? "following" : "followers";
            var (people, status) = await GetAsync<List<CommunityPersonUIVM>>($"api/community/profiles/{Uri.EscapeDataString(userId)}/{path}");
            if (status == HttpStatusCode.Unauthorized) return Unauthorized();

            return PartialView("_People", people ?? new List<CommunityPersonUIVM>());
        }
    }
}
