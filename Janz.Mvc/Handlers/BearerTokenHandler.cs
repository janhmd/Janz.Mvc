using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Janz.Mvc.Handlers
{
    // API-yə gedən hər sorğuya: istifadəçi daxil olubsa, cookie-də saxlanmış JWT-ni Authorization başlığı kimi əlavə edir
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is not null)
            {
                var token = await context.GetTokenAsync("access_token");
                if (!string.IsNullOrEmpty(token))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
