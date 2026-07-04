using System.ComponentModel.DataAnnotations;

namespace YemekMenuProjem.Models
{
    public class Kategori
    {
    [Key]
        public int Id { get; set; }
        public string KategoriAdi { get; set; }

        public List<Menu> Menuler { get; set; }
    }
}
