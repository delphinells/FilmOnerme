using Microsoft.AspNetCore.Identity;

namespace FilmOnerme.Models
{
    public class ApplicationUser : IdentityUser
    {
        public UserProfile Profile { get; set; }
    }
}
