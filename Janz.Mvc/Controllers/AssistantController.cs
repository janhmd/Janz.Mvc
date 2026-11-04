using Janz.Mvc.ViewModels.Assistant;
using Microsoft.AspNetCore.Mvc;

namespace Janz.Mvc.Controllers
{
    // Sayt köməkçisi (fetch): sualı API-yə (Gemini) ötürür. Cavab boşdursa sayt hazır cavablarına qayıdır
    public class AssistantController : ApiControllerBase
    {
        public AssistantController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory, configuration)
        {
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ask(string? question)
        {
            var (result, _, _) = await PostAsync<AssistantAnswerUIVM>("api/assistant", new { question });

            return Ok(new { answer = result?.Answer ?? string.Empty });
        }
    }
}
