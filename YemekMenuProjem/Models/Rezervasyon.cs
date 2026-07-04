using System.ComponentModel.DataAnnotations;

namespace YemekMenuProjem.Models
{
    public class Rezervasyon
    {
        [Key]
        public int Id { get; set; }

        public string MusteriAdSoyad { get; set; }
        public string Telefon { get; set; }

        public DateTime RezervasyonTarihi { get; set; }
        public int KisiSayisi { get; set; }

        public int MasaId { get; set; }
        public Masa Masa { get; set; }

        public string Notlar { get; set; }
    }
}
