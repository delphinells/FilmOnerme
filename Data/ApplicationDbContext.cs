using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FilmOnerme.Models;

namespace FilmOnerme.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Film> Films { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserFilmStatus> UserFilmStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(p => p.UserId);
        }
    }
}
