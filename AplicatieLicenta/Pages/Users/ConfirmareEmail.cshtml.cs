using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System;
using System.Data.SqlClient;

namespace AplicatieLicenta.Pages.Users
{
    public class ConfirmareEmailModel : PageModel
    {
        [BindProperty] public string email { get; set; }
        [BindProperty] public string cod_confirmare { get; set; }
        public string Message { get; set; }

        private readonly IConfiguration _configuration;

        public ConfirmareEmailModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(cod_confirmare) || string.IsNullOrEmpty(email))
            {
                Message = "Email-ul ?i codul de confirmare sunt obligatorii.";
                return Page();
            }

            // Verificare utilizator în stocarea temporarã
            if (!TemporaryRegistrationStore.PendingRegistrations.TryGetValue(email, out var userData))
            {
                Message = "Email-ul nu este înregistrat sau codul de confirmare este invalid.";
                return Page();
            }

            // Validare cod de confirmare
            if (userData.CodConfirmare != cod_confirmare)
            {
                Message = "Codul de confirmare este invalid.";
                return Page();
            }

            // Inserare utilizator în baza de date
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"
                    INSERT INTO Users (email, parola, tip_utilizator, categorie_varsta, email_confirmat)
                    VALUES (@Email, @Parola, @TipUtilizator, @CategorieVarsta, 1)";
                var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Parola", userData.HashedPassword);
                command.Parameters.AddWithValue("@TipUtilizator", userData.TipUtilizator);
                command.Parameters.AddWithValue("@CategorieVarsta", userData.CategorieVarsta);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            // Eliminare utilizator din stocarea temporarã
            TemporaryRegistrationStore.PendingRegistrations.Remove(email);

            // Redirec?ionare cãtre pagina de login cu un mesaj de succes
            return RedirectToPage("/Login", new { Message = "Email confirmat! Te po?i autentifica." });
        }

        public async Task<IActionResult> OnPostResendAsync()
        {
            if (string.IsNullOrEmpty(email))
            {
                Message = "Email-ul este obligatoriu pentru retrimiterea codului.";
                return Page();
            }

            // Verificare utilizator în stocarea temporarã
            if (!TemporaryRegistrationStore.PendingRegistrations.TryGetValue(email, out var userData))
            {
                Message = "Nu am gãsit un utilizator neconfirmat cu acest email.";
                return Page();
            }

            // Generare nou cod de confirmare
            var newCode = GenerateConfirmationCode();
            userData = (userData.HashedPassword, userData.TipUtilizator, userData.CategorieVarsta, newCode);
            TemporaryRegistrationStore.PendingRegistrations[email] = userData;

            // Trimitere nou email cu codul de confirmare
            await SendConfirmationEmail(email, newCode);

            Message = "Un nou cod de confirmare a fost trimis pe email.";
            return Page();
        }

        private string GenerateConfirmationCode()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async Task SendConfirmationEmail(string email, string confirmationCode)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

            var mailMessage = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress(smtpSettings.SenderEmail),
                Subject = "Confirmare email",
                Body = $"Codul tãu de confirmare este: {confirmationCode}",
                IsBodyHtml = false,
            };

            mailMessage.To.Add(email);

            using (var smtpClient = new System.Net.Mail.SmtpClient(smtpSettings.Server, smtpSettings.Port))
            {
                smtpClient.Credentials = new System.Net.NetworkCredential(smtpSettings.SenderEmail, smtpSettings.SenderPassword);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        public class SmtpSettings
        {
            public string Server { get; set; }
            public int Port { get; set; }
            public string SenderEmail { get; set; }
            public string SenderPassword { get; set; }
        }
    }
}
