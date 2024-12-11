using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;

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
            if (string.IsNullOrEmpty(cod_confirmare))
            {
                Message = "Codul este obligatoriu.";
                return Page();
            }

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                // Verificã dacã codul existã ?i email-ul nu a fost confirmat
                var query = "SELECT id_utilizator FROM Users WHERE cod_confirmare = @CodConfirmare AND email_confirmat = 0";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CodConfirmare", cod_confirmare);

                await connection.OpenAsync();
                var userId = await command.ExecuteScalarAsync();

                if (userId == null)
                {
                    Message = "Cod invalid sau email deja confirmat.";
                    return Page();
                }

                // Actualizeazã starea utilizatorului (email_confirmat = 1)
                var updateQuery = "UPDATE Users SET email_confirmat = 1, cod_confirmare = NULL WHERE id_utilizator = @UserId";
                var updateCommand = new SqlCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("@UserId", userId);

                await updateCommand.ExecuteNonQueryAsync();
            }

            // Redirec?ionare cãtre pagina de login cu un mesaj de succes
            return RedirectToPage("/Login", new { Message = "Email confirmat! Te po?i autentifica." });
        }

        public async Task<IActionResult> OnPostResendAsync()
        {
            if (string.IsNullOrEmpty(email))
            {
                Message = "Nu am putut gãsi email-ul asociat.";
                return Page();
            }

            // Generare nou cod de confirmare
            var newCode = GenerateConfirmationCode();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = "UPDATE Users SET cod_confirmare = @NewCode WHERE email = @Email AND email_confirmat = 0";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NewCode", newCode);
                command.Parameters.AddWithValue("@Email", email);

                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    Message = "Nu am putut gãsi un utilizator neconfirmat cu acest email.";
                    return Page();
                }
            }

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
