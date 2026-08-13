using System.Text;
using System.Text.Json;

namespace GestionParcMateriel.API.Services
{
    public interface IAiAnalysisService
    {
        Task<string> AnalyserParcAsync(string donneesResume);
    }

    public class AiAnalysisService : IAiAnalysisService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AiAnalysisService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["Anthropic:ApiKey"] ?? "";
        }

        public async Task<string> AnalyserParcAsync(string donneesResume)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                return "Clé API non configurée.";

            var requestBody = new
            {
                model = "claude-sonnet-4-5",
                max_tokens = 500,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = $@"Tu es un assistant IT pour SOPAL. Voici l'état actuel du parc matériel informatique :

{donneesResume}

Rédige une courte analyse en français (5-8 lignes maximum) avec :
1. Un résumé de l'état général du parc
2. 2-3 recommandations concrètes et actionnables pour le technicien IT

Reste concis, professionnel, et pratique. Pas de formules de politesse, va droit au but."
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
            request.Headers.Add("x-api-key", _apiKey);
            request.Headers.Add("anthropic-version", "2023-06-01");
            request.Content = content;

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return $"Erreur API IA : {response.StatusCode}";

                using var doc = JsonDocument.Parse(responseBody);
                var texte = doc.RootElement
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetString();

                return texte ?? "Aucune réponse générée.";
            }
            catch (Exception ex)
            {
                return $"Erreur lors de l'appel à l'IA : {ex.Message}";
            }
        }
    }
}