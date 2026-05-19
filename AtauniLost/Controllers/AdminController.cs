using AtauniLost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System; // Hata yakalama için gerekli

namespace AtauniLost.Controllers
{
    public class AdminController : Controller
    {
        private readonly AtauniLostDbContext _context = new AtauniLostDbContext();

        // Admin Kontrolü: Güvenlik için e-posta kontrolü yapar
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Email") == "admin@atauni.edu.tr";
        }

        // 1. TÜM İLANLARI LİSTELEME
        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var tumIlanlar = _context.Ilanlars.OrderByDescending(i => i.Tarih).ToList();
            return View(tumIlanlar);
        }

        // 2. ONAY BEKLEYENLERİ FİLTRELEME
        public IActionResult OnayBekleyenler()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var bekleyenler = _context.Ilanlars.Where(i => i.OnayDurumu == false).ToList();
            return View("Index", bekleyenler);
        }

        // 3. İLAN ONAYLAMA
        [HttpPost]
        public IActionResult Onayla(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var ilan = _context.Ilanlars.Find(id);
            if (ilan != null)
            {
                ilan.OnayDurumu = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // 4. İLAN SİLME
        [HttpPost]
        public IActionResult Sil(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var ilan = _context.Ilanlars.Find(id);
            if (ilan != null)
            {
                var yorumlar = _context.Yorumlars.Where(y => y.IlanId == id).ToList();
                _context.Yorumlars.RemoveRange(yorumlar);

                _context.Ilanlars.Remove(ilan);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // 5. İLAN GÜNCELLEME SAYFASI (GET)
        public IActionResult Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var ilan = _context.Ilanlars.Find(id);
            if (ilan == null) return RedirectToAction("Index");

            return View(ilan);
        }

        // 6. İLAN GÜNCELLEME İŞLEMİ (POST) - Siyah Sayfa Hatası Önleyici Güncelleme
        [HttpPost]
        public IActionResult Edit(Ilanlar guncelIlan)
        {
            // Güvenlik Kontrolü
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            try
            {
                var varOlanIlan = _context.Ilanlars.Find(guncelIlan.IlanId);
                if (varOlanIlan != null)
                {
                    // Bilgileri Güncelle
                    varOlanIlan.Baslik = guncelIlan.Baslik;
                    varOlanIlan.Aciklama = guncelIlan.Aciklama;
                    varOlanIlan.Kategori = guncelIlan.Kategori;
                    varOlanIlan.Durum = guncelIlan.Durum;
                    varOlanIlan.Konum = guncelIlan.Konum;
                    varOlanIlan.OnayDurumu = guncelIlan.OnayDurumu;

                    _context.SaveChanges();
                }

                // İşlem başarılıysa listeye dön
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Siyah sayfa yerine hatayı ekrana basar (Geliştirme aşaması için)
                return Content("Güncelleme sırasında bir hata oluştu: " + ex.Message);
            }
        }
    }
}