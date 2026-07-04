using Microsoft.EntityFrameworkCore;
using YemekMenuProjem.Models;

namespace YemekMenuProjem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Masa> Masalar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Menu> Menuler { get; set; }
        public DbSet<Rezervasyon> Rezervasyonlar { get; set; }
    }
}