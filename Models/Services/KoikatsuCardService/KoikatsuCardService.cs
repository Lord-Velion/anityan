using System.Text.Json.Serialization;
using static AniTyan.Models.Services.KoikatsuCardService.KoikatsuCardService;

namespace AniTyan.Models.Services.KoikatsuCardService
{
    public interface IKoikatsuCardService
    {
        Task<bool> IsValidCardAsync(IFormFile cardFile);
        Task<CharacterNameResponse> GetCharacterNameAsync(IFormFile cardFile);
    }

    public class KoikatsuCardService : IKoikatsuCardService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KoikatsuCardService> _logger;

        public KoikatsuCardService(HttpClient httpClient, ILogger<KoikatsuCardService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> IsValidCardAsync(IFormFile cardFile)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await cardFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(memoryStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(cardFile.ContentType);

                content.Add(fileContent, "file", cardFile.FileName);

                var response = await _httpClient.PostAsync("validate", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Koikatsu service returned {StatusCode}: {Error}", response.StatusCode, errorContent);
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<ValidationResponse>();
                return result?.IsValid == true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CRITICAL: Error calling koikatsu card service");
                return false;
            }
        }

        public async Task<CharacterNameResponse> GetCharacterNameAsync(IFormFile cardFile)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await cardFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(memoryStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(cardFile.ContentType);
                content.Add(fileContent, "file", cardFile.FileName);

                var response = await _httpClient.PostAsync("name", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Koikatsu service returned {StatusCode} for name extraction: {Error}",
                        response.StatusCode, errorContent);

                    throw new HttpRequestException($"Name extraction failed: {response.StatusCode} - {errorContent}");
                }

                var result = await response.Content.ReadFromJsonAsync<CharacterNameResponse>();
                if (result == null)
                {
                    throw new InvalidOperationException("Deserialization returned null for character name");
                }
                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "CRITICAL: Error in GetCharacterNameAsync");
                throw;
            }
        }

        public class ValidationResponse
        {
            [JsonPropertyName("is_valid")]
            public bool IsValid { get; set; }
            public string? Error { get; set; }
        }

        public class CharacterNameResponse
        {
            [JsonPropertyName("lastname")]
            public string LastName { get; set; } = string.Empty;

            [JsonPropertyName("firstname")]
            public string FirstName { get; set; } = string.Empty;

            [JsonPropertyName("nickname")]
            public string Nickname { get; set; } = string.Empty;
        }
    }
}
