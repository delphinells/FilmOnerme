using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FilmOnerme.Data;
using FilmOnerme.Models;
using Microsoft.AspNetCore.Identity;

namespace FilmOnerme.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = user.Id,
                    FullName = User.Identity?.Name ?? "",
                    FavoriteGenres = "",
                    Bio = "",
                    FavoriteDirector = "",
                    WatchingFrequency = "",
                    ProfilePicture = "default-avatar.png"
                };
                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }

            ViewBag.Appointments = await _context.Appointments
                .Include(a => a.Film)
                .Where(a => a.KullaniciAdi == User.Identity.Name)
                .OrderByDescending(a => a.Tarih)
                .Take(5)
                .ToListAsync();

            return View(profile);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfile updatedProfile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || updatedProfile.UserId != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProfile = await _context.UserProfiles
                        .FirstOrDefaultAsync(p => p.Id == updatedProfile.Id);

                    if (existingProfile == null)
                    {
                        return NotFound();
                    }

                    // Update only the editable fields
                    existingProfile.FullName = updatedProfile.FullName;
                    existingProfile.Bio = updatedProfile.Bio;
                    existingProfile.FavoriteGenres = updatedProfile.FavoriteGenres;
                    existingProfile.FavoriteDirector = updatedProfile.FavoriteDirector;
                    existingProfile.WatchingFrequency = updatedProfile.WatchingFrequency;
                    existingProfile.UpdatedAt = DateTime.Now;

                    // Only update profile picture if a new one is provided
                    if (!string.IsNullOrEmpty(updatedProfile.ProfilePicture) && 
                        updatedProfile.ProfilePicture != existingProfile.ProfilePicture)
                    {
                        existingProfile.ProfilePicture = updatedProfile.ProfilePicture;
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Profil başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.UserProfiles.AnyAsync(p => p.Id == updatedProfile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(updatedProfile);
        }
    }
} 