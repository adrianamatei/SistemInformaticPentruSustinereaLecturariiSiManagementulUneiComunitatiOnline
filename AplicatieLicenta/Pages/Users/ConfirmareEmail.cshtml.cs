using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System;
using System.Data.SqlClient;
using AplicatieLicenta.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace AplicatieLicenta.Pages.Users
{
    public class ConfirmareEmailModel : PageModel
    {
        [BindProperty]
        public string email { get; set; }
        [BindProperty]
        public string cod_confirmare { get; set; }
        public string Message { get; set; }
        private readonly IConfiguration _configuration;
        public ConfirmareEmailModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(cod_confirmare))
            {
                Message = "Email-ul si codul de confirmare sunt obligatorii !";
                return Page();
            }
            if (!TemporaryRegistrationStore.PendingRegistrations.TryGetValue(email, out var userData))
            {
                Message = "Email-ul nu este inregistrat sau codul de confirmare este invalid !";
                return Page();
            }
            if (userData.CodConfirmare != cod_confirmare)
            {
                Message = "Codul de confirmare este invalid !";
                return Page();
            }
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"INSERT INTO Users(email,parola,tip_utilizator,categorie_varsta,email_confirmat) VALUES (@Email,@Parola,@TipUtilizator,@CategorieVarsta,1)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Parola", userData.HashedPassword);
                command.Parameters.AddWithValue("@TipUtilizator", userData.TipUtilizator);
                command.Parameters.AddWithValue("@CategorieVarsta", userData.CategorieVarsta);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            TemporaryRegistrationStore.PendingRegistrations.Remove(email);
            return RedirectToPage("/Login", new { Message = "Email confirmat ! Te poti autentifica !" });


        }
        public async Task<IActionResult> OnPostResendAsync()
        {
            if (string.IsNullOrEmpty(email))
            {
                Message = "Email-ul este obligatoriu pentru retrimiterea codului !";
                return Page();
            }
            if (!TemporaryRegistrationStore.PendingRegistrations.TryGetValue(email, out var userData))
            {
                Message = "Nu am gasit un utilizator neconfirmat cu acest email !";
                return Page();
            }
            var newCode = GenerateConfirmationCode();
            userData = (userData.HashedPassword, userData.TipUtilizator, userData.CategorieVarsta, newCode);
            TemporaryRegistrationStore.PendingRegistrations[email] = userData;
            await SendConfirmationEmail(email, newCode);
            Message = "Un nou cod de confirmare a fost trimis pe email !";
            return Page();
        }
        private string GenerateConfirmationCode()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWYZ0123456789";
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private async Task SendConfirmationEmail(string email, string confirmationCode)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
            var mailMessage = new System.Net.Mail.MailMessage()
            {
                From = new System.Net.Mail.MailAddress(smtpSettings.SenderEmail),
                Subject = "Confirmare email",
                Body = $"Codul tau de confirmare este : {confirmationCode}",
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
    }
}
