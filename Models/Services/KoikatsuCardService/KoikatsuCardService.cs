using System.Text.Json.Serialization;

namespace AniTyan.Models.Services.KoikatsuCardService
{
    public interface IKoikatsuCardService
    {
        Task<bool> IsValidCardAsync(IFormFile cardFile);
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

        public class ValidationResponse
        {
            [JsonPropertyName("is_valid")]
            public bool IsValid { get; set; }
            public string? Error { get; set; }
        }
    }
}
