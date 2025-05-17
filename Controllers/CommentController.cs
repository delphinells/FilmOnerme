using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FilmOnerme.Data;
using FilmOnerme.Models;
using Microsoft.AspNetCore.Identity;

namespace FilmOnerme.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Yorum ekleme
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(int filmId, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return BadRequest("Yorum içeriği boş olamaz.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var film = await _context.Films.FindAsync(filmId);
            if (film == null)
            {
                return NotFound("Film bulunamadı.");
            }

            var comment = new Comment
            {
                Content = content,
                FilmId = filmId,
                UserId = user.Id,
                UserName = user.UserName,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Film", new { id = filmId });
        }

        // Yorum silme
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Sadece yorum sahibi veya admin silebilir
            if (comment.UserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Film", new { id = comment.FilmId });
        }

        // Yorum düzenleme
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, string content)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Sadece yorum sahibi veya admin düzenleyebilir
            if (comment.UserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            comment.Content = content;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Film", new { id = comment.FilmId });
        }
    }
} 