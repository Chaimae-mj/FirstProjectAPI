using System.Text;
using System.Text.Json;
using FirstProjectAPI.Dtos.Ai;
using FirstProjectAPI.Infra;
using Microsoft.EntityFrameworkCore;

namespace FirstProjectAPI.Services.Ai
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly MyContext _context;
        private readonly string _apiKey;
        private readonly string _model;

        public AiService(HttpClient httpClient, MyContext context, IConfiguration config)
        {
            _httpClient = httpClient;
            _context = context;

            var geminiSection = config.GetSection("Gemini");
            _apiKey = geminiSection["ApiKey"]
                ?? throw new InvalidOperationException("Clé API Gemini manquante dans appsettings.json (section Gemini:ApiKey).");
            _model = geminiSection["Model"] ?? "gemini-2.5-flash";
        }

        public async Task<string> AskAsync(string question)
        {
            var prompt =
                "Tu es un formateur IA pédagogique pour une plateforme de formation en ligne. " +
                "Réponds de façon claire, concise et bienveillante à la question suivante posée par un apprenant :\n\n" +
                question;

            return await CallGeminiAsync(prompt);
        }

        public async Task<ExplainModuleResponseDto> ExplainModuleAsync(ExplainModuleDto dto)
        {
            var module = await _context.Modules
                .Include(m => m.Modalites)
                .FirstOrDefaultAsync(m => m.Id == dto.IdModule);

            if (module == null)
                throw new ModuleIntrouvableException();

            var sb = new StringBuilder();
            sb.AppendLine("Tu es un formateur IA pédagogique. Explique le module de formation suivant à un apprenant, de façon claire et structurée.");
            sb.AppendLine();
            sb.AppendLine($"Titre du module : {module.Titre}");
            sb.AppendLine();

            if (module.Modalites.Any())
            {
                sb.AppendLine("Contenu du module :");
                foreach (var modalite in module.Modalites.OrderBy(m => m.Ordre))
                {
                    sb.AppendLine($"- [{modalite.Type}] {modalite.Titre}");
                    if (!string.IsNullOrWhiteSpace(modalite.Contenu))
                        sb.AppendLine($"  Contenu : {modalite.Contenu}");
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.QuestionSpecifique))
            {
                sb.AppendLine();
                sb.AppendLine($"Question précise de l'apprenant sur ce module : {dto.QuestionSpecifique}");
            }

            var explication = await CallGeminiAsync(sb.ToString());

            return new ExplainModuleResponseDto
            {
                IdModule = module.Id,
                ModuleTitre = module.Titre,
                Explication = explication
            };
        }

        private async Task<string> CallGeminiAsync(string prompt)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent";

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-goog-api-key", _apiKey);

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new GeminiApiException($"Erreur Gemini ({(int)response.StatusCode}) : {responseContent}");

            using var doc = JsonDocument.Parse(responseContent);

            if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            {
                throw new GeminiApiException(
                    "Gemini n'a renvoyé aucune réponse exploitable (possiblement bloquée par les filtres de sécurité).");
            }

            var text = candidates[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "L'IA n'a renvoyé aucune réponse exploitable.";
        }
    }
}