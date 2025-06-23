using AplicatieLicenta.Data;
using AplicatieLicenta.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http.Features;
using Azure.Core;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace AplicatieLicenta.Pages.Users
{
    public class StartUserModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public StartUserModel(AppDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string CategorieVarsta { get; set; }
        public List<CluburiLectura> CluburiRelevante { get; set; } = new();
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string AvatarImagine { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToPage("/Login");

            UserId = userId.Value;

            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
                return RedirectToPage("/Login");

            CategorieVarsta = user.CategorieVarsta;
            UserEmail = user.Email;

            
            var rezultate = await _context.RezultateQuiz
                .Where(r => r.UserId == UserId && r.Scor >= 80)
                .ToListAsync();

            int scorTotal = rezultate.Sum(r =>
                r.Scor >= 100 ? 20 :
                r.Scor >= 90 ? 15 : 10);

            int pieseObt = rezultate.Count;
            string avatar = "reading.png";
            int totalPiese = 3;

            if (pieseObt >= 9) 
            { 
                avatar = "astronaut.png";
                totalPiese = 11;
            }
            else if (pieseObt >= 7) 
            { 
                avatar = "magician.png"; 
                totalPiese = 9;
            }
            else if (pieseObt >= 5)
            {
                avatar = "explorer.png";
                totalPiese = 7; 
            }
            else if (pieseObt >= 3)
            {
                avatar = "detective.png"; 
                totalPiese = 5; 
            }
            AvatarImagine = avatar;
            ViewData["AvatarCurent"] = System.IO.Path.GetFileNameWithoutExtension(avatar).CapitalizeFirst();
            ViewData["Piese"] = pieseObt;
            ViewData["TotalPiese"] = totalPiese;
            ViewData["ScorTotal"] = scorTotal;

            
            ViewData["TopUtilizatori"] = await _context.Users
                .Select(u => new
                {
                    u.Email,
                    Scor = _context.RezultateQuiz
                        .Where(r => r.UserId == u.IdUtilizator && r.Scor >= 80)
                        .Select(r => r.Scor >= 100 ? 20 : r.Scor >= 90 ? 15 : 10)
                        .Sum()
                })
                .OrderByDescending(u => u.Scor)
                .Take(10)
                .ToListAsync();

            CluburiRelevante = await _context.CluburiLectura
                .Include(c => c.MembriClub)
                .Where(c => c.CategorieVarsta == CategorieVarsta)
                .ToListAsync();

            return Page();
        }


        public async Task<IActionResult> OnPostJoinAsync(int clubId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToPage("/Login");

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
                return RedirectToPage("/Login");

            var club = await _context.CluburiLectura.FirstOrDefaultAsync(c => c.IdClub == clubId);
            if (club == null)
                return NotFound();

            bool esteMembru = await _context.MembriClub
                .AnyAsync(m => m.IdUtilizator == userId.Value && m.IdClub == clubId);

            if (!esteMembru)
            {
                _context.MembriClub.Add(new MembriClub
                {
                    IdUtilizator = userId.Value,
                    IdClub = clubId,
                    DataInscrierii = DateTime.Now,
                    Status = "In asteptare"
                });

                await _context.SaveChangesAsync();
                string adminEmail = "ionelaamatei2004@gmail.com"; 
                string subject = $"Cerere nouã pentru clubul: {club.Nume}";
                string linkAdmin = Url.Page("/Admin/GestionareCluburiLectura", null, null, Request.Scheme);
                string body = $@"
            <p><strong>{user.Email}</strong> a trimis o cerere de înscriere în clubul <strong>{club.Nume}</strong>.</p>
            <p>Pentru a gestiona cererea, acceseazã: <a href='{linkAdmin}'>Gestionare Cluburi de Lecturã</a></p>";

                await SendGenericEmail(adminEmail, subject, body);
            }

            TempData["Success"] = "Cererea ta a fost trimisã cãtre admin!";
            return RedirectToPage();
        }


        public async Task<JsonResult> OnGetIncarcaMesajeAsync(int idClub)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return new JsonResult(new { error = "Utilizator neautentificat" });

            var club = await _context.CluburiLectura.FindAsync(idClub);
            if (club != null)
            {
                _context.UsersActivity.Add(new UsersActivity
                {
                    UserId = userId.Value,
                    Action = $"A vizualizat conversatia din clubul de lectura \"{club.Nume}\"",
                    Data = $"Utilizatorul {UserEmail} a accesat chatul",
                    Timestamp = DateTime.Now
                });

                await _context.SaveChangesAsync();
            }

            var mesaje = await _context.MesajClub
                .Include(m => m.Utilizator)
                .Where(m => m.IdClub == idClub)
                .OrderBy(m => m.DataTrimiterii)
                .Select(m => new
                {
                    email = m.Utilizator.Email,
                    continut = m.Continut,
                    dataTrimiterii = m.DataTrimiterii.ToString("HH:mm"),
                    audioUrl = string.IsNullOrEmpty(m.UrlFisierAudio) ? null : m.UrlFisierAudio
                })
                .ToListAsync();

            return new JsonResult(mesaje);
        }


        [IgnoreAntiforgeryToken]
        [RequestSizeLimit(100_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 100_000_000)]
        public async Task<IActionResult> OnPostVocalAsync()
        {
            try
            {
                var vocal = Request.Form.Files["vocal"];
                var idClub = Convert.ToInt32(Request.Form["idClub"]);
                var userId = Convert.ToInt32(Request.Form["userId"]);
                var userEmail = Request.Form["userEmail"];

                if (vocal == null || vocal.Length == 0)
                    return BadRequest("Fi?ierul audio nu a fost primit corect!");

                var extensie = Path.GetExtension(vocal.FileName) ?? ".webm";
                var fileName = $"{Guid.NewGuid()}{extensie}";
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "vocale");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var path = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await vocal.CopyToAsync(stream);
                }

                _context.UsersActivity.Add(new UsersActivity
                {
                    UserId = userId,
                    Action = $"A trimis un mesaj vocal în clubul cu ID {idClub}",
                    Data = $"Fi?ier: {fileName}",
                    Timestamp = DateTime.Now
                });

                _context.MesajClub.Add(new MesajClub
                {
                    IdClub = idClub,
                    IdUtilizator = userId,
                    Continut = "Audio",
                    UrlFisierAudio = $"/vocale/{fileName}",
                    DataTrimiterii = DateTime.Now
                });

                await _context.SaveChangesAsync();

                return Content(fileName);
            }
            catch (Exception ex)
            {
                return BadRequest("Eroare internã: " + ex.Message);
            }
        }
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostTrimiteTextAsync()
        {
            try
            {
                var userId = Convert.ToInt32(Request.Form["userId"]);
                var userEmail = Request.Form["userEmail"];
                var mesaj = Request.Form["mesaj"];
                var idClub = Convert.ToInt32(Request.Form["idClub"]);

                if (string.IsNullOrWhiteSpace(mesaj))
                    return BadRequest("Mesajul nu poate fi gol.");

                _context.MesajClub.Add(new MesajClub
                {
                    IdClub = idClub,
                    IdUtilizator = userId,
                    Continut = mesaj,
                    UrlFisierAudio = null,
                    DataTrimiterii = DateTime.Now
                });

                _context.UsersActivity.Add(new UsersActivity
                {
                    UserId = userId,
                    Action = $"A trimis un mesaj text în clubul cu ID {idClub}",
                    Data = $"Continut: {mesaj}",
                    Timestamp = DateTime.Now
                });

                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest("Eroare internã: " + ex.Message);
            }
        }
        private async Task SendGenericEmail(string email, string subject, string htmlBody)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

            var mailMessage = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress(smtpSettings.SenderEmail),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            using (var smtpClient = new System.Net.Mail.SmtpClient(smtpSettings.Server, smtpSettings.Port))
            {
                smtpClient.Credentials = new System.Net.NetworkCredential(smtpSettings.SenderEmail, smtpSettings.SenderPassword);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        public async Task<IActionResult> OnGetFavoriteAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return Content("<p>Trebuie sã fii logat pentru a vedea cãrtile favorite.</p>", "text/html");

            var carti = await _context.CartiPreferate
                .Where(f => f.IdUtilizator == userId.Value)
                .Include(f => f.Carte)
                .Select(f => f.Carte)
                .ToListAsync();

            if (!carti.Any())
                return Content("<p>Nu ai adãugat nicio carte la favorite.</p>", "text/html");

            var pdf = carti.Where(c => c.TipCarte == "PDF").ToList();
            var audio = carti.Where(c => c.TipCarte == "Audio").ToList();

            var htmlBuilder = new System.Text.StringBuilder();

            if (pdf.Any())
            {
                htmlBuilder.Append("<h6 style='color:#5a422a;'>CARTI PDF:</h6><ul>");
                foreach (var carte in pdf)
                {
                    htmlBuilder.Append($@"
                <li style='margin-bottom: 0.5rem;'>
                    <a href='/Users/DetaliiPdf?id={carte.IdCarte}' 
                       style='text-decoration: none; color: #8b0000; font-weight: bold;'>
                        {carte.Titlu}
                    </a>
                </li>");
                }
                htmlBuilder.Append("</ul>");
            }

            if (audio.Any())
            {
                htmlBuilder.Append("<h6 style='color:#5a422a;'>CARTI AUDIO:</h6><ul>");
                foreach (var carte in audio)
                {
                    htmlBuilder.Append($@"
                <li style='margin-bottom: 0.5rem;'>
                    <a href='/Users/DetaliiAudio?id={carte.IdCarte}' 
                       style='text-decoration: none; color: #8b0000; font-weight: bold;'>
                        {carte.Titlu}
                    </a>
                </li>");
                }
                htmlBuilder.Append("</ul>");
            }

            return Content(htmlBuilder.ToString(), "text/html");
        }




    }


    public static class StringExtensions
    {
        public static string CapitalizeFirst(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return char.ToUpper(value[0]) + value.Substring(1);
        }
    }
   

}
