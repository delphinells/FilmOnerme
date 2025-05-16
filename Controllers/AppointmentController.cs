using FilmOnerme.Data;
using FilmOnerme.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FilmOnerme.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Randevuları listeleme
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("admin"))
            {
                var all = await _context.Appointments
                    .Include(a => a.Film)
                    .ToListAsync();

                return View(all);
            }

            var userName = User.Identity?.Name;

            var userAppointments = await _context.Appointments
                .Where(a => a.KullaniciAdi == userName)
                .Include(a => a.Film)
                .ToListAsync();

            return View(userAppointments);
        }

        // Randevu oluşturma formu (GET)
        public async Task<IActionResult> Create(int? filmId)
        {
            if (filmId.HasValue)
            {
                var film = await _context.Films.FindAsync(filmId.Value);
                if (film != null)
                {
                    ViewData["FilmId"] = film.Id;
                    ViewData["FilmAdi"] = film.Baslik;
                    return View();
                }
            }

            ViewBag.FilmList = new SelectList(await _context.Films.ToListAsync(), "Id", "Baslik");
            return View();
        }

        // Randevu kaydetme (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            appointment.KullaniciAdi = User.Identity?.Name;

            if (ModelState.IsValid)
            {
                // Aynı film için aynı tarihte başka randevu var mı kontrol et
                var existingAppointment = await _context.Appointments
                    .AnyAsync(a => a.FilmId == appointment.FilmId && 
                                 a.Tarih.Date == appointment.Tarih.Date &&
                                 a.Tarih.Hour == appointment.Tarih.Hour);

                if (existingAppointment)
                {
                    ModelState.AddModelError("Tarih", "Bu film için seçilen tarih ve saatte başka bir randevu bulunmaktadır.");
                    ViewBag.FilmList = new SelectList(await _context.Films.ToListAsync(), "Id", "Baslik");
                    return View(appointment);
                }

                _context.Appointments.Add(appointment);

                var film = await _context.Films.FindAsync(appointment.FilmId);
                if (film != null)
                {
                    film.RandevuSayisi += 1;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.FilmList = new SelectList(await _context.Films.ToListAsync(), "Id", "Baslik");
            return View(appointment);
        }

        // Randevu detayları
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Film)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            // Sadece admin veya randevunun sahibi görebilir
            if (!User.IsInRole("admin") && appointment.KullaniciAdi != User.Identity?.Name)
                return Forbid();

            return View(appointment);
        }

        // Randevu silme sayfası (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Film)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            // Sadece admin veya randevunun sahibi silebilir
            if (!User.IsInRole("admin") && appointment.KullaniciAdi != User.Identity?.Name)
                return Forbid();

            return View(appointment);
        }

        // Randevu silme işlemi (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Film)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            // Sadece admin veya randevunun sahibi silebilir
            if (!User.IsInRole("admin") && appointment.KullaniciAdi != User.Identity?.Name)
                return Forbid();

            // Film randevu sayısını güncelle
            if (appointment.Film != null)
            {
                appointment.Film.RandevuSayisi = Math.Max(0, appointment.Film.RandevuSayisi - 1);
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GEÇİCİ: JSON çıktısı (debug)
        public async Task<IActionResult> DebugAppointments()
        {
            var all = await _context.Appointments.Include(a => a.Film).ToListAsync();
            return Json(all);
        }
    }
}
