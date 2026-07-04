using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using YemekMenuProjem.Models;

namespace YemekMenuProjem.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext dbcontext;
        public CategoryController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            var result = dbcontext.Kategoriler.ToList();
            return View(result);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Kategori kategori)
        {

            dbcontext.Kategoriler.Add(kategori);
            dbcontext.SaveChanges();
            return RedirectToAction("Index");

        }
        public IActionResult Edit(int id)
        {
            var result = dbcontext.Kategoriler.Find(id);
            return View(result);
        }
        [HttpPost]
        public IActionResult Edit(Kategori kategori)
        {
            dbcontext.Update(kategori);
            dbcontext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var result = dbcontext.Kategoriler.Find(id);

            if (result != null)
            {
                dbcontext.Kategoriler.Remove(result);
                dbcontext.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
