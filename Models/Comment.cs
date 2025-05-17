using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmOnerme.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Yorum içeriği zorunludur.")]
        [Display(Name = "Yorum")]
        public string Content { get; set; }

        [Display(Name = "Yorum Tarihi")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Film ile ilişki
        public int FilmId { get; set; }
        public Film Film { get; set; }

        // Kullanıcı ile ilişki
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }
    }
} 