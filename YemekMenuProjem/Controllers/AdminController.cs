using Microsoft.AspNetCore.Mvc;
using YemekMenuProjem.Models;

namespace YemekMenuProjem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext dbcontext;

        public AdminController(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            var result = dbcontext.Masalar.ToList();
            return View(result);
        }
    }
}