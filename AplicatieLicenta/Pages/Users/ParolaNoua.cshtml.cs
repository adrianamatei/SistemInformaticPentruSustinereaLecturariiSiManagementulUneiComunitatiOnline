using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using BCrypt.Net;

namespace AplicatieLicenta.Pages.Users
{
    public class ParolaNouaModel : PageModel
    {
        [BindProperty]
        public string Token { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }
        public string ErrorMessage { get; set; }

        private readonly IConfiguration _configuration;

        public ParolaNouaModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet(string token)
        {
            Token = token; // Set the token value from the query string
        }

        public async Task<IActionResult> OnPostResetPasswordAsync()
        {
            // Validate if passwords match
            if (string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
            {
                ErrorMessage = "Toate campurile sunt obligatorii.";
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Parolele nu se potrivesc.";
                return Page();
            }

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                // Validate the token and its expiration
                var query = @"
                    SELECT id_utilizator 
                    FROM Users 
                    WHERE TokenResetare = @Token AND ExpirareToken > @Now";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Token", Token);
                    command.Parameters.AddWithValue("@Now", DateTime.Now);

                    var userId = await command.ExecuteScalarAsync();
                    if (userId == null)
                    {
                        ErrorMessage = "Token-ul este invalid sau a expirat.";
                        return Page();
                    }

                    // Update the password and clear the reset token
                    query = @"
                        UPDATE Users 
                        SET parola = @NewPassword, TokenResetare = NULL, ExpirareToken = NULL 
                        WHERE id_utilizator = @UserId";
                    using (var updateCommand = new SqlCommand(query, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@NewPassword", BCrypt.Net.BCrypt.HashPassword(NewPassword));
                        updateCommand.Parameters.AddWithValue("@UserId", (int)userId);

                        await updateCommand.ExecuteNonQueryAsync();
                    }
                }
            }

            // Set the success message and redirect
            Message = "Parola a fost resetatã cu succes.";
            return RedirectToPage("/Login");
        }
    }
}
