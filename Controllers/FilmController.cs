using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FilmOnerme.Data;
using FilmOnerme.Models;
using System.Diagnostics;

namespace FilmOnerme.Controllers
{
    public class FilmController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FilmController> _logger;

        public FilmController(ApplicationDbContext context, ILogger<FilmController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Film
        public async Task<IActionResult> Index()
        {
            var films = await _context.Films
                .OrderByDescending(f => f.GosterimTarihi)
                .ToListAsync();

            return View(films);
        }

        // GET: Film/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .FirstOrDefaultAsync(m => m.Id == id);

            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // GET: Film/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Film/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Baslik,Aciklama,FilmTuru,Yonetmen,YapimYili,ImdbPuani,GosterimTarihi")] Film film)
        {
            try
            {
                _logger.LogInformation("Film ekleme işlemi başlatıldı");
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model doğrulama hataları:");
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            _logger.LogWarning($"Hata: {error.ErrorMessage}");
                            ModelState.AddModelError(string.Empty, error.ErrorMessage);
                        }
                    }
                    return View(film);
                }

                _logger.LogInformation($"Film bilgileri: Başlık={film.Baslik}, Yönetmen={film.Yonetmen}, Yıl={film.YapimYili}");
                
                film.RandevuSayisi = 0;
                _context.Add(film);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Film başarıyla eklendi. ID: {film.Id}");
                TempData["SuccessMessage"] = "Film başarıyla eklendi.";
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Film eklenirken hata oluştu: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Film eklenirken bir hata oluştu: {ex.Message}");
                return View(film);
            }
        }

        // GET: Film/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }
            return View(film);
        }

        // POST: Film/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Baslik,Aciklama,FilmTuru,Yonetmen,YapimYili,ImdbPuani,GosterimTarihi,RandevuSayisi")] Film film)
        {
            if (id != film.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Film başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.Id))
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
            return View(film);
        }

        // GET: Film/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .FirstOrDefaultAsync(m => m.Id == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Film/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film != null)
            {
                _context.Films.Remove(film);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Film başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Film/CreateAppointment/5
        [Authorize]
        public async Task<IActionResult> CreateAppointment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .FirstOrDefaultAsync(m => m.Id == id);

            if (film == null)
            {
                return NotFound();
            }

            var appointment = new Appointment
            {
                FilmId = film.Id,
                KullaniciAdi = User.Identity?.Name ?? "",
                Tarih = DateTime.Now.AddDays(1).Date.AddHours(14)
            };

            return View(appointment);
        }

        // POST: Film/CreateAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CreateAppointment([Bind("Id,FilmId,KullaniciAdi,Tarih")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                // Randevu saatini kontrol et (14:00 - 22:00 arası)
                if (appointment.Tarih.Hour < 14 || appointment.Tarih.Hour >= 22)
                {
                    ModelState.AddModelError("Tarih", "Randevu saati 14:00 - 22:00 arasında olmalıdır.");
                    return View(appointment);
                }

                // Aynı gün ve saatte başka randevu var mı kontrol et
                var existingAppointment = await _context.Appointments
                    .AnyAsync(a => a.Tarih == appointment.Tarih);

                if (existingAppointment)
                {
                    ModelState.AddModelError("Tarih", "Bu tarih ve saatte başka bir randevu bulunmaktadır.");
                    return View(appointment);
                }

                // Kullanıcının aynı filme daha önce randevu alıp almadığını kontrol et
                var userHasAppointment = await _context.Appointments
                    .AnyAsync(a => a.FilmId == appointment.FilmId && 
                                 a.KullaniciAdi == User.Identity.Name);

                if (userHasAppointment)
                {
                    ModelState.AddModelError("", "Bu film için zaten bir randevunuz bulunmaktadır.");
                    return View(appointment);
                }

                appointment.KullaniciAdi = User.Identity?.Name ?? "";
                _context.Add(appointment);

                // Filmin randevu sayısını artır
                var film = await _context.Films.FindAsync(appointment.FilmId);
                if (film != null)
                {
                    film.RandevuSayisi++;
                    _context.Update(film);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Randevunuz başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(appointment);
        }

        // GET: Film/Appointments
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Appointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Film)
                .OrderBy(a => a.Tarih)
                .ToListAsync();
            return View(appointments);
        }

        // GET: Film/DeleteAppointment/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAppointment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Film)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Film/DeleteAppointment/5
        [HttpPost, ActionName("DeleteAppointment")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAppointmentConfirmed(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Film)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment != null)
            {
                // Filmin randevu sayısını azalt
                if (appointment.Film != null)
                {
                    appointment.Film.RandevuSayisi--;
                    _context.Update(appointment.Film);
                }

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Randevu başarıyla silindi.";
            }

            return RedirectToAction(nameof(Appointments));
        }

        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.Id == id);
        }
    }
} 