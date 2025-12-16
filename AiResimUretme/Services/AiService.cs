using System.Net.Http;
using System.Threading.Tasks;

namespace AiResimUretme.Services
{
    public class AiService
    {
        private readonly HttpClient _httpClient;

        public AiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> ResimUret(string prompt)
        {
            // Türkçe karakter sorununu önlemek için prompt'u URL uyumlu hale getir
            var duzgunPrompt = System.Uri.EscapeDataString(prompt);

            // Pollinations.ai servisine istek at (Model: Flux - daha gerçekçi)
            var url = $"https://image.pollinations.ai/prompt/{duzgunPrompt}?model=flux";

            // Resmi indir ve byte dizisi olarak geri döndür
            return await _httpClient.GetByteArrayAsync(url);
        }
    }
}