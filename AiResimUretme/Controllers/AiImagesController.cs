using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiResimUretme.Data;
using AiResimUretme.Models;
using AiResimUretme.Services; // Yeni servisimizi buraya ekledik

namespace AiResimUretme.Controllers
{
    public class AiImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AiService _aiService; // Yeni servisi tanımladık

        // Constructor (Yapıcı Metot) Güncellendi
        public AiImagesController(ApplicationDbContext context, AiService aiService)
        {
            _context = context;
            _aiService = aiService; // Servisi içeri aldık
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
                    // 1. Yeni Servisi Çağır (Resim byte olarak gelir)
                    byte[] imageBytes = await _aiService.ResimUret(aiImage.Prompt);

                    // 2. Gelen resmi Base64 formatına çevir (Veritabanı için)
                    string base64Image = Convert.ToBase64String(imageBytes);
                    aiImage.ImageUrl = $"data:image/jpeg;base64,{base64Image}";

                    // 3. Tarihi ekle ve Kaydet
                    aiImage.CreatedDate = DateTime.Now;

                    _context.Add(aiImage);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Resim üretilirken hata oluştu: " + ex.Message);
                }
            }
            return View(aiImage);
        }

        // About sayfası (Genelde HomeController'da olur ama buradaysa kalsın)
        public IActionResult About()
        {
            return View();
        }

        // --- ESKİ CallAiApi METODU TAMAMEN SİLİNDİ ---
        // Artık o işi AiService yapıyor.

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