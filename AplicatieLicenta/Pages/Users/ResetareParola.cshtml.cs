using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AplicatieLicenta.Models;

namespace AplicatieLicenta.Pages.Users
{
    public class ResetareParolaModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        public string Message { get; set; }
        public string ErrorMessage { get; set; }

        private readonly IConfiguration _configuration;

        public ResetareParolaModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnPostRequestResetAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                ErrorMessage = "Va rugam sa introduceti o adresa de email !";
                return Page();
            }

            string resetToken;
            DateTime? expirationTime;

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                var selectQuery = "SELECT id_utilizator, TokenResetare, ExpirareToken FROM Users WHERE email = @Email";
                using (var selectCommand = new SqlCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@Email", Email);

                    using (var reader = await selectCommand.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                        {
                            ErrorMessage = "Email-ul nu este asociat unui cont existent !";
                            return Page();
                        }

                        await reader.ReadAsync();

                        var userId = reader.GetInt32(0);
                        resetToken = reader["TokenResetare"] as string;
                        expirationTime = reader["ExpirareToken"] as DateTime?;
                        if (!string.IsNullOrEmpty(resetToken) && expirationTime.HasValue && expirationTime > DateTime.Now)
                        {
                            Message = "Un email a fost deja trimis.";
                            return Page();
                        }
                        resetToken = Guid.NewGuid().ToString();
                        expirationTime = DateTime.Now.AddHours(1);
                        reader.Close();
                        var updateQuery = @" UPDATE Users SET TokenResetare = @Token, ExpirareToken = @ExpirationDate WHERE id_utilizator = @UserId";

                        using (var updateCommand = new SqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@UserId", userId);
                            updateCommand.Parameters.AddWithValue("@Token", resetToken);
                            updateCommand.Parameters.AddWithValue("@ExpirationDate", expirationTime);

                            await updateCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            await SendResetEmail(Email, resetToken);

            Message = "Un email cu instructiuni de resetare a fost trimis !";
            return Page();
        }

        private async Task SendResetEmail(string email, string token)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

            var resetLink = Url.Page("/Users/ParolaNoua", null, new { token = token }, Request.Scheme);

            var mailMessage = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress(smtpSettings.SenderEmail),
                Subject = "Resetare parola",
                Body = $"Pentru a reseta parola, accesati linkul urmator: <a href='{resetLink}'>Resetare parola</a>",
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
    }
}
