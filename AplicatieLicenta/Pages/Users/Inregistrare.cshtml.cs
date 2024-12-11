using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BCrypt.Net; // Asigurã-te cã ai instalat pachetul BCrypt.Net

namespace AplicatieLicenta.Pages.Users
{
    public class InregistrareModel : PageModel
    {
        [BindProperty] public string email { get; set; }
        [BindProperty] public string parola { get; set; }
        [BindProperty] public string ConfirmareParola { get; set; }
        [BindProperty] public string tip_utilizator { get; set; }
        [BindProperty] public string categorie_varsta { get; set; }
        public string Message { get; set; }

        private readonly IConfiguration _configuration;

        public InregistrareModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GenerateConfirmationCode()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validare parolã
            if (parola != ConfirmareParola)
            {
                Message = "Parolele nu coincid!";
                return Page();
            }

            // Criptare parolã
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(parola);

            // Generare cod de confirmare
            var cod_confirmare = GenerateConfirmationCode();

            // Salvare în baza de date
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"
                    INSERT INTO Users (email, parola, tip_utilizator, categorie_varsta, email_confirmat, cod_confirmare)
                    VALUES (@Email, @Parola, @TipUtilizator, @CategorieVarsta, 0, @CodConfirmare)";
                var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Parola", hashedPassword);
                command.Parameters.AddWithValue("@TipUtilizator", tip_utilizator);
                command.Parameters.AddWithValue("@CategorieVarsta", categorie_varsta);
                command.Parameters.AddWithValue("@CodConfirmare", cod_confirmare);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            // Trimitere email
            await SendConfirmationEmail(email, cod_confirmare);

            // Redirec?ionare la pagina de confirmare email
            return RedirectToPage("ConfirmareEmail", new { email = email });
        }

        private async Task SendConfirmationEmail(string email, string cod_confirmare)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

            var mailMessage = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress(smtpSettings.SenderEmail),
                Subject = "Confirmare email",
                Body = $"Codul tãu de confirmare este: {cod_confirmare}",
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
