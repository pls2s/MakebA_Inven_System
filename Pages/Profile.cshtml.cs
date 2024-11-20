using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace MakebA_Inven_System.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly string _connectionString = "Server=tcp:makebafinal.database.windows.net,1433;Initial Catalog=makeba;Persist Security Info=False;User ID=makeba;Password=Pls2s0727;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public UserModel UserProfile { get; set; } = new UserModel();

        public string Message { get; set; }

        public string LoggedInUserEmail { get; set; }

        public void OnGet()
        {
            // Get logged-in user's email from claims
            LoggedInUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(LoggedInUserEmail))
            {
                Message = "No logged-in user found.";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT FirstName, LastName, Username, Email, Department, Phone
                        FROM Users
                        WHERE Email = @Email";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", LoggedInUserEmail);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UserProfile.FirstName = reader.GetString(0);
                                UserProfile.LastName = reader.GetString(1);
                                UserProfile.Username = reader.GetString(2);
                                UserProfile.Email = reader.GetString(3);
                                UserProfile.Department = reader.GetString(4);
                                UserProfile.Phone = reader.GetString(5);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"An error occurred: {ex.Message}";
            }
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync();
            return RedirectToPage("/Login");
        }

        public class UserModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Department { get; set; }
            public string Phone { get; set; }
        }
    }
}
