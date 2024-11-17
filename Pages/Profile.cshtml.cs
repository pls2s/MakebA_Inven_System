using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace MakebA_Inven_System.Pages
{
	public class ProfileModel : PageModel
	{
		public UserProfile Profile { get; set; } = new UserProfile(); // Renamed from "User" to "Profile"

		private readonly string _connectionString = "Server=tcp:your_server.database.windows.net,1433;Initial Catalog=your_database;Persist Security Info=False;User ID=your_user;Password=your_password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

		public async Task<IActionResult> OnGetAsync()
		{
			// Use the built-in User.Identity.Name to fetch the logged-in user's username
			string loggedInUsername = User?.Identity?.Name;

			if (string.IsNullOrEmpty(loggedInUsername))
			{
				// If no user is logged in, redirect to the login page
				return RedirectToPage("/Login");
			}

			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();
				var query = @"
                    SELECT Username, Email, Role, DateJoined 
                    FROM Users 
                    WHERE Username = @Username";

				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@Username", loggedInUsername);

					using (var reader = await command.ExecuteReaderAsync())
					{
						if (await reader.ReadAsync())
						{
							Profile.Username = reader.GetString(0);
							Profile.Email = reader.GetString(1);
							Profile.Role = reader.GetString(2);
							Profile.DateJoined = reader.GetDateTime(3);
						}
						else
						{
							// Redirect to error page if the user is not found in the database
							return RedirectToPage("/Error");
						}
					}
				}
			}

			return Page();
		}

		public class UserProfile
		{
			public string Username { get; set; } = string.Empty;
			public string Email { get; set; } = string.Empty;
			public string Role { get; set; } = string.Empty;
			public DateTime DateJoined { get; set; } = DateTime.MinValue;
		}
	}
}
