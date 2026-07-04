using System.ComponentModel.DataAnnotations;

namespace YemekMenuProjem.Models
{
    public class Menu
    {
        [Key]
        public int Id { get; set; }
        public string YemekAdi { get; set; }
        public string Aciklama { get; set; }
        public decimal Fiyat { get; set; }
        public string ResimUrl { get; set; }

        public int KategoriId { get; set; }
        public Kategori Kategori { get; set; }
    }
}
