using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmOnerme.Models
{
    public class UserFilmStatus
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public int FilmId { get; set; }
        [ForeignKey("FilmId")]
        public Film Film { get; set; }

        [Required]
        public FilmStatus Status { get; set; } // İzledim veya İzleyeceğim

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum FilmStatus
    {
        Watched = 1,
        ToWatch = 2
    }
} 