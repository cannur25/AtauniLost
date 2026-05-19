using AtauniLost.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Session için gerekli
using System.Linq;

namespace AtauniLost.Controllers
{
    public class AccountController : Controller
    {
        // Bağlantı nesnesi
        private readonly AtauniLost.Models.AtauniLostDbContext _context = new AtauniLost.Models.AtauniLostDbContext();

        // 1. KAYIT SAYFASI (GET)
        public IActionResult Register()
        {
            return View();
        }

        // 1. KAYIT SAYFASI (POST)
        [HttpPost]
        public IActionResult Register(Kullanicilar yeniKullanici)
        {
            if (ModelState.IsValid)
            {
                _context.Kullanicilars.Add(yeniKullanici);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(yeniKullanici);
        }

        // 2. GİRİŞ SAYFASI (GET)
        public IActionResult Login()
        {
            return View();
        }

        // 2. GİRİŞ SAYFASI (POST) - ÖZEL YÖNLENDİRME EKLENDİ
        [HttpPost]
        public IActionResult Login(string email, string sifre)
        {
            var kullanici = _context.Kullanicilars
                                    .FirstOrDefault(k => k.Email == email && k.Sifre == sifre);

            if (kullanici != null)
            {
                // OTURUM BİLGİLERİNİ KAYDEDİYORUZ (Session)
                HttpContext.Session.SetString("KullaniciId", kullanici.KullaniciId.ToString());
                HttpContext.Session.SetString("AdSoyad", kullanici.AdSoyad);
                HttpContext.Session.SetString("Email", kullanici.Email); // E-postayı yönlendirme kontrolleri için saklıyoruz

                // --- ADMİN ÖZEL YÖNLENDİRMESİ ---
                // Eğer giriş yapan senin belirlediğin admin hesabıysa doğrudan onay paneline gönder
                if (kullanici.Email == "admin@atauni.edu.tr")
                {
                    return RedirectToAction("OnayBekleyenler", "Admin");
                }

                // Normal kullanıcı ise ana sayfaya gönder
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Hata = "E-posta veya şifre hatalı!";
            return View();
        }

        // 3. ÇIKIŞ İŞLEMİ
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Tüm oturum bilgilerini siler
            return RedirectToAction("Login");
        }

        // 4. PROFİL / İLANLARIM SAYFASI
        public IActionResult Profil()
        {
            var kullaniciIdStr = HttpContext.Session.GetString("KullaniciId");

            if (string.IsNullOrEmpty(kullaniciIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            int kullaniciId = int.Parse(kullaniciIdStr);

            var kullaniciBilgisi = _context.Kullanicilars.FirstOrDefault(k => k.KullaniciId == kullaniciId);

            var kullaniciIlanlari = _context.Ilanlars
                                            .Where(i => i.KullaniciId == kullaniciId)
                                            .OrderByDescending(i => i.Tarih)
                                            .ToList();

            ViewBag.Kullanici = kullaniciBilgisi;

            return View(kullaniciIlanlari);
        }
    }
}