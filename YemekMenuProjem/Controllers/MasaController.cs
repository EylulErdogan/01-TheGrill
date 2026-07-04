using Microsoft.AspNetCore.Mvc;
using YemekMenuProjem.Models;

namespace YemekMenuProjem.Controllers
{
    public class MasaController : Controller
    {
        private readonly ApplicationDbContext dbcontext;

        public MasaController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            var result = dbcontext.Masalar.ToList();
            return View(result);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Masa masa)
        {

            dbcontext.Masalar.Add(masa);
            dbcontext.SaveChanges();
            return RedirectToAction("Index");

        }
        public IActionResult Edit(int id)
        {
            var result = dbcontext.Masalar.Find(id);
            return View(result);
        }
        [HttpPost]
        public IActionResult Edit(Masa masa)
        {
            dbcontext.Update(masa);
            dbcontext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var result = dbcontext.Masalar.Find(id);

            if (result != null)
            {
                dbcontext.Masalar.Remove(result);
                dbcontext.SaveChanges();
            }

            return RedirectToAction("Index");
        }




    }
}