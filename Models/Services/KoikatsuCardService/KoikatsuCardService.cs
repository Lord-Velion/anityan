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
            using var content = new MultipartFormDataContent();
            using var stream = cardFile.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.
                MediaTypeHeaderValue(cardFile.ContentType);
            content.Add(fileContent, "file", cardFile.FileName);

            try
            {
                var response = await _httpClient.PostAsync("validate", content);
                if (!response.IsSuccessStatusCode)
                    return false;

                var result = await response.Content.ReadFromJsonAsync<ValidationResponse>();
                return result?.IsValid == true;
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling koikatsu card service");
                return false;
            }
        }

        public async Task<CharacterNameResponse> GetCharacterNameAsync(IFormFile cardFile)
        {
            using var content = new MultipartFormDataContent();
            using var stream = cardFile.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.
                MediaTypeHeaderValue(cardFile.ContentType);
            content.Add(fileContent, "file", cardFile.FileName);

            try
            {
                var response = await _httpClient.PostAsync("name", content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorText = await _httpClient.PostAsync("name", content);
                    _logger.LogError("Failed to get character name: {StatusCode}, {Error}",
                        response.StatusCode, errorText);

                    throw new HttpRequestException($"Name extraction failed: {response.StatusCode}");
                }

                var result = await response.Content.ReadFromJsonAsync<CharacterNameResponse>();
                if (result == null)
                {
                    throw new InvalidOperationException("Deserialization returned null");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling koikatsu card service for name extraction");
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
