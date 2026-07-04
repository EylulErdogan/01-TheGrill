using System.ComponentModel.DataAnnotations;

namespace YemekMenuProjem.Models
{
    public class Masa
    {
        [Key]
        public int Id { get; set; }
        public string MasaNo { get; set; }
        public int KisiKapasitesi { get; set; }
        public bool MusaitMi { get; set; }
    }
}
