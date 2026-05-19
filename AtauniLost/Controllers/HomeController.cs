using System.Diagnostics;
using AtauniLost.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AtauniLost.Controllers
{
    public class HomeController : Controller
    {
        // 1. ADIM: Veritabaný baðlantý nesnesini ekliyoruz
        private readonly AtauniLostDbContext _context = new AtauniLostDbContext();

        // 2. ADIM: Onay Filtresi ve Arama mantýðý eklenmiþ Index metodu
        public IActionResult Index(string search)
        {
            // ÖNEMLÝ GÜNCELLEME: Sadece 'OnayDurumu' true olan ilanlarý sorguya dahil ediyoruz.
            // Böylece yönetici onaylamadan hiçbir ilan ana sayfaya düþmez.
            var ilanlarSorgu = _context.Ilanlars.Where(i => i.OnayDurumu == true).AsQueryable();

            // Eðer kullanýcý arama kutusuna bir þey yazdýysa
            if (!string.IsNullOrEmpty(search))
            {
                // Aramayý küçük harfe çeviriyoruz (Büyük/küçük harf duyarlýlýðýný aþmak için)
                string aranan = search.ToLower();

                // Baþlýkta, Açýklamada veya Konumda aranan kelime geçiyor mu?
                ilanlarSorgu = ilanlarSorgu.Where(i => i.Baslik.ToLower().Contains(aranan)
                                                   || i.Aciklama.ToLower().Contains(aranan)
                                                   || i.Konum.ToLower().Contains(aranan));

                // Arayüzde "Cüzdan için sonuçlar" yazýsý görünsün diye bilgi gönderiyoruz
                ViewBag.AramaSonucu = $"'{search}' için bulunan sonuçlar listeleniyor";
            }

            // Filtrelenmiþ ve ONAYLANMIÞ tüm sonuçlarý en yeni tarihliden baþlayarak listele
            var ilanlar = ilanlarSorgu.OrderByDescending(i => i.Tarih).ToList();

            return View(ilanlar);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}