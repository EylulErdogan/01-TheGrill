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
            ViewBag.ToplamRezervasyon = dbcontext.Rezervasyonlar.Count();
            ViewBag.ToplamMasa = dbcontext.Masalar.Count();
            ViewBag.MusaitMasa = dbcontext.Masalar.Count(x => x.MusaitMi);
            ViewBag.DoluMasa = dbcontext.Masalar.Count(x => !x.MusaitMi);

            var gunlukRapor = dbcontext.Rezervasyonlar
                .GroupBy(x => x.RezervasyonTarihi.Date)
                .Select(x => new
                {
                    Tarih = x.Key.ToString("dd.MM.yyyy"),
                    Sayi = x.Count()
                })
                .ToList();

            ViewBag.GunlukTarihler = JsonSerializer.Serialize(gunlukRapor.Select(x => x.Tarih));
            ViewBag.GunlukSayilar = JsonSerializer.Serialize(gunlukRapor.Select(x => x.Sayi));

            var masaRapor = dbcontext.Rezervasyonlar
                .Include(x => x.Masa)
                .GroupBy(x => x.Masa.MasaNo)
                .Select(x => new
                {
                    MasaNo = x.Key,
                    Sayi = x.Count()
                })
                .ToList();

            ViewBag.MasaNolar = JsonSerializer.Serialize(masaRapor.Select(x => x.MasaNo));
            ViewBag.MasaSayilar = JsonSerializer.Serialize(masaRapor.Select(x => x.Sayi));

            return View();
        }
    }
}
