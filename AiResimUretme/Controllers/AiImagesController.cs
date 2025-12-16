using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiResimUretme.Data;
using AiResimUretme.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AiResimUretme.Controllers
{
    public class AiImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AiImagesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var images = await _context.AiImages
                                       .OrderByDescending(i => i.CreatedDate)
                                       .ToListAsync();
            return View(images);
        }

        public IActionResult Create()
        {
            return View();
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AiImage aiImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string generatedImage = await CallAiApi(aiImage.Prompt);

                    aiImage.ImageUrl = generatedImage;
                    aiImage.CreatedDate = DateTime.Now;

                    _context.Add(aiImage);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Hata: " + ex.Message);
                }
            }
            return View(aiImage);
        }

        public IActionResult About()
        {
            return View();
        }

        private async Task<string> CallAiApi(string prompt)
        {
 
            string? apiUrl = _configuration["AiSettings:ApiUrl"];
            string? apiKey = _configuration["AiSettings:ApiKey"];

            if (string.IsNullOrEmpty(apiUrl) || string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("API Ayarları bulunamadı. Lütfen appsettings.json dosyasını kontrol edin.");
            }

            using (var client = new HttpClient())
            {
                var cleanToken = apiKey.Replace("Bearer ", "").Trim();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cleanToken);

                var requestData = new
                {
                    inputs = prompt,
                    options = new { wait_for_model = true }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API Hatası ({response.StatusCode}): {errorContent}");
                }

                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                string base64Image = Convert.ToBase64String(imageBytes);
                return $"data:image/jpeg;base64,{base64Image}";
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var image = await _context.AiImages.FirstOrDefaultAsync(m => m.Id == id);
            if (image == null) return NotFound();

            return View(image);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.AiImages.FindAsync(id);
            if (image != null)
            {
                _context.AiImages.Remove(image);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var image = await _context.AiImages.FindAsync(id);
            if (image == null) return NotFound();

            return View(image);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AiImage aiImage)
        {
            if (id != aiImage.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aiImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.AiImages.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(aiImage);
        }
    }
}