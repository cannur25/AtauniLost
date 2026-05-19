using AtauniLost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AtauniLost.Controllers
{
    public class IlanController : Controller
    {
        private readonly AtauniLostDbContext _context = new AtauniLostDbContext();

        // 1. İLAN VERME SAYFASI (GET)
        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("KullaniciId")))
            {
                return RedirectToAction("Login", "Account");
            }
            return View("~/Views/Ilan/Create.cshtml");
        }

        // 2. İLAN KAYDETME (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Ilanlar yeniIlan, IFormFile? Fotograf)
        {
            var kullaniciIdStr = HttpContext.Session.GetString("KullaniciId");
            if (!string.IsNullOrEmpty(kullaniciIdStr))
            {
                if (Fotograf != null && Fotograf.Length > 0)
                {
                    var dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(Fotograf.FileName);
                    var yuklemeYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(yuklemeYolu)) Directory.CreateDirectory(yuklemeYolu);
                    var tamYol = Path.Combine(yuklemeYolu, dosyaAdi);
                    using (var stream = new FileStream(tamYol, FileMode.Create))
                    {
                        await Fotograf.CopyToAsync(stream);
                    }
                    yeniIlan.FotografYolu = "/uploads/" + dosyaAdi;
                }
                yeniIlan.KullaniciId = int.Parse(kullaniciIdStr);
                yeniIlan.Tarih = DateTime.Now;
                yeniIlan.OnayDurumu = false;
                _context.Ilanlars.Add(yeniIlan);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View("~/Views/Ilan/Create.cshtml", yeniIlan);
        }

        // 3. İLAN DETAY SAYFASI (GET)
        public IActionResult Details(int id)
        {
            var ilan = _context.Ilanlars.FirstOrDefault(i => i.IlanId == id);
            if (ilan == null) return RedirectToAction("Index", "Home");
            return View("~/Views/Ilan/Details.cshtml", ilan);
        }

        // --- YORUM EKLEME (POST) ---
        [HttpPost]
        public IActionResult YorumEkle(int ilanId, string icerik)
        {
            var kullaniciIdStr = HttpContext.Session.GetString("KullaniciId");

            if (!string.IsNullOrEmpty(icerik) && !string.IsNullOrEmpty(kullaniciIdStr))
            {
                var yeniYorum = new Yorumlar
                {
                    IlanId = ilanId,
                    KullaniciId = int.Parse(kullaniciIdStr),
                    Icerik = icerik,
                    Tarih = DateTime.Now
                };

                _context.Yorumlars.Add(yeniYorum);
                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = ilanId });
        }

        // ==========================================
        // 4. İLAN SİLME (POST) - GÜNCELLENDİ (HATA ÇÖZÜLDÜ)
        // ==========================================
        [HttpPost]
        public IActionResult Sil(int id)
        {
            var kullaniciIdStr = HttpContext.Session.GetString("KullaniciId");
            var ilan = _context.Ilanlars.FirstOrDefault(i => i.IlanId == id);

            if (ilan != null && kullaniciIdStr != null && ilan.KullaniciId == int.Parse(kullaniciIdStr))
            {
                // HATA ÇÖZÜMÜ: Önce bu ilana ait tüm yorumları siliyoruz
                var bagliYorumlar = _context.Yorumlars.Where(y => y.IlanId == id).ToList();
                _context.Yorumlars.RemoveRange(bagliYorumlar);

                // Sonra ilanın kendisini siliyoruz
                _context.Ilanlars.Remove(ilan);

                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Details", new { id = id });
        }

        // ==========================================
        // 5. İLAN DÜZENLEME SAYFASI (GET)
        // ==========================================
        public IActionResult Edit(int id)
        {
            var kullaniciIdStr = HttpContext.Session.GetString("KullaniciId");
            var ilan = _context.Ilanlars.FirstOrDefault(i => i.IlanId == id);

            if (ilan != null && kullaniciIdStr != null && ilan.KullaniciId == int.Parse(kullaniciIdStr))
            {
                return View("~/Views/Ilan/Edit.cshtml", ilan);
            }

            return RedirectToAction("Index", "Home");
        }

        // ==========================================
        // 6. İLAN GÜNCELLEME (POST)
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> Edit(Ilanlar guncelIlan, IFormFile? Fotograf)
        {
            var kullaniciIdStr = HttpContext.Session.GetString("KullaniciId");
            var varOlanIlan = _context.Ilanlars.FirstOrDefault(i => i.IlanId == guncelIlan.IlanId);

            if (varOlanIlan != null && kullaniciIdStr != null && varOlanIlan.KullaniciId == int.Parse(kullaniciIdStr))
            {
                varOlanIlan.Baslik = guncelIlan.Baslik;
                varOlanIlan.Aciklama = guncelIlan.Aciklama;
                varOlanIlan.Kategori = guncelIlan.Kategori;
                varOlanIlan.Durum = guncelIlan.Durum;
                varOlanIlan.Konum = guncelIlan.Konum;

                if (Fotograf != null && Fotograf.Length > 0)
                {
                    var dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(Fotograf.FileName);
                    var yuklemeYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    var tamYol = Path.Combine(yuklemeYolu, dosyaAdi);

                    using (var stream = new FileStream(tamYol, FileMode.Create))
                    {
                        await Fotograf.CopyToAsync(stream);
                    }
                    varOlanIlan.FotografYolu = "/uploads/" + dosyaAdi;
                }

                _context.SaveChanges();
                return RedirectToAction("Details", new { id = varOlanIlan.IlanId });
            }

            return RedirectToAction("Index", "Home");
        }
    }
}