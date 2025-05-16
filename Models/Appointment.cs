using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmOnerme.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public string KullaniciAdi { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Tarih { get; set; }

        [Required]
        public int FilmId { get; set; }

        [ForeignKey("FilmId")]
        [Display(Name = "Film")]
        public Film? Film { get; set; } // ← BURASI required OLMAMALI
    }
}
