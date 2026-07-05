using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using YemekMenuProjem.Models;

namespace YemekMenuProjem.Controllers
{
    public class RaporController : Controller
    {
        private readonly ApplicationDbContext dbcontext;
        public RaporController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }
        public IActionResult Index()
        {
            // Kartlar
            ViewBag.ToplamRezervasyon = dbcontext.Rezervasyonlar.Count();
            ViewBag.ToplamMasa = dbcontext.Masalar.Count();
            ViewBag.MusaitMasa = dbcontext.Masalar.Count(x => x.MusaitMi);
            ViewBag.DoluMasa = dbcontext.Masalar.Count(x => !x.MusaitMi);

            ViewBag.BugunkuRezervasyon = dbcontext.Rezervasyonlar
                .Count(x => x.RezervasyonTarihi.Date == DateTime.Today);

            ViewBag.BugunkuKisi = dbcontext.Rezervasyonlar
                .Where(x => x.RezervasyonTarihi.Date == DateTime.Today)
                .Sum(x => (int?)x.KisiSayisi) ?? 0;

            ViewBag.OrtalamaKisi = dbcontext.Rezervasyonlar.Any()
                ? Math.Round(dbcontext.Rezervasyonlar.Average(x => x.KisiSayisi), 1)
                : 0;

            ViewBag.EnCokRezervasyonAlanMasa = dbcontext.Rezervasyonlar
                .Include(x => x.Masa)
                .GroupBy(x => x.Masa.MasaNo)
                .OrderByDescending(x => x.Count())
                .Select(x => "Masa " + x.Key)
                .FirstOrDefault() ?? "-";

            // Günlere göre rezervasyon
            var gunlukRapor = dbcontext.Rezervasyonlar
     .GroupBy(x => x.RezervasyonTarihi.Date)
     .Select(x => new
     {
         Tarih = x.Key,
         Sayi = x.Count()
     })
     .OrderBy(x => x.Tarih)
     .ToList();

            ViewBag.GunlukTarihler = JsonSerializer.Serialize(
                gunlukRapor.Select(x => x.Tarih.ToString("dd.MM"))
            );

            ViewBag.GunlukSayilar = JsonSerializer.Serialize(
                gunlukRapor.Select(x => x.Sayi)
            );
            // Masalara göre rezervasyon
            var masaRapor = dbcontext.Rezervasyonlar
                .Include(x => x.Masa)
                .GroupBy(x => x.Masa.MasaNo)
                .Select(x => new
                {
                    Masa = "Masa " + x.Key,
                    Sayi = x.Count()
                })
                .ToList();

            ViewBag.MasaNolar = JsonSerializer.Serialize(masaRapor.Select(x => x.Masa));
            ViewBag.MasaSayilar = JsonSerializer.Serialize(masaRapor.Select(x => x.Sayi));

            // Son rezervasyonlar
            ViewBag.SonRezervasyonlar = dbcontext.Rezervasyonlar
                .Include(x => x.Masa)
                .OrderByDescending(x => x.RezervasyonTarihi)
                .Take(5)
                .ToList();

            return View();
        }
    }
}
