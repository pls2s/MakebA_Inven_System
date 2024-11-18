using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MakebA_Inven_System.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public string Message { get; set; }

        private readonly string _connectionString = "Server=tcp:makebafinal.database.windows.net,1433;Initial Catalog=makeba;Persist Security Info=False;User ID=makeba;Password=Pls2s0727;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string hashedPassword = HashPassword(Input.Password);

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Query to verify username and fetch email
                    string query = @"
                        SELECT Email FROM Users 
                        WHERE Username = @Username 
                        AND PasswordHash = @PasswordHash";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", Input.Username);
                        command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                        var email = await command.ExecuteScalarAsync() as string;

                        if (!string.IsNullOrEmpty(email))
                        {
                            // Store email in claims for later use
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, Input.Username),
                                new Claim(ClaimTypes.Email, email)
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = true // Keep the user logged in across requests
                            };

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                            // Redirect to the inbox or desired page
                            return RedirectToPage("/Inbox");
                        }
                        else
                        {
                            Message = "Invalid username or password.";
                            return Page();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = "An error occurred: " + ex.Message;
                return Page();
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Username is required.")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            public string Password { get; set; }
        }
    }
}
