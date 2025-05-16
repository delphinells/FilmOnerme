using System.ComponentModel.DataAnnotations;

namespace FilmOnerme.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        [Display(Name = "Ad Soyad")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ad Soyad en az 2, en fazla 100 karakter olmalıdır.")]
        public string FullName { get; set; }

        [Display(Name = "Profil Resmi")]
        public string ProfilePicture { get; set; } = "default-avatar.png";

        [Required(ErrorMessage = "Favori Film Türü seçimi zorunludur.")]
        [Display(Name = "Favori Film Türü")]
        public string FavoriteGenres { get; set; }

        [Display(Name = "Biyografi")]
        [StringLength(500, ErrorMessage = "Biyografi en fazla 500 karakter olabilir.")]
        public string? Bio { get; set; }

        [Required(ErrorMessage = "En sevdiğiniz yönetmeni belirtmeniz zorunludur.")]
        [Display(Name = "En Sevdiği Yönetmen")]
        [StringLength(100, ErrorMessage = "Yönetmen adı en fazla 100 karakter olabilir.")]
        public string FavoriteDirector { get; set; }

        [Required(ErrorMessage = "Film izleme sıklığını seçmeniz zorunludur.")]
        [Display(Name = "Film İzleme Sıklığı")]
        public string WatchingFrequency { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
} 