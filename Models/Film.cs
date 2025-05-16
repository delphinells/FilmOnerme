using System.ComponentModel.DataAnnotations;

namespace FilmOnerme.Models
{
    public class Film
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Film adı zorunludur.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Film adı en az 2, en fazla 100 karakter olmalıdır.")]
        [Display(Name = "Film Adı")]
        public string Baslik { get; set; }

        [Required(ErrorMessage = "Film açıklaması zorunludur.")]
        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; }

        [Required(ErrorMessage = "Yönetmen adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Yönetmen adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Yönetmen")]
        public string Yonetmen { get; set; }

        [Required(ErrorMessage = "Yapım yılı zorunludur.")]
        [Range(1900, 2099, ErrorMessage = "Yapım yılı 1900-2099 arasında olmalıdır.")]
        [Display(Name = "Yapım Yılı")]
        public int YapimYili { get; set; }

        [Required(ErrorMessage = "Film türü zorunludur.")]
        [Display(Name = "Tür")]
        public string FilmTuru { get; set; }

        [Required(ErrorMessage = "IMDB puanı zorunludur.")]
        [Range(0, 10, ErrorMessage = "IMDB puanı 0-10 arasında olmalıdır.")]
        [Display(Name = "IMDB Puanı")]
        public decimal ImdbPuani { get; set; }

        [Required(ErrorMessage = "Gösterim tarihi zorunludur.")]
        [Display(Name = "Gösterim Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime GosterimTarihi { get; set; }

        public int RandevuSayisi { get; set; } = 0;
    }
}
