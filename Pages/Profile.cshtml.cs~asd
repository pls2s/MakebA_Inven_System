using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient; // Import for SqlClient
using System;
using System.Threading.Tasks;

namespace MakebA_Inven_System.Pages
{
	public class ProfileModel : PageModel
	{
		public UserProfile User { get; set; }

		private readonly string _connectionString = "Server=tcp:your_server.database.windows.net,1433;Initial Catalog=your_database;Persist Security Info=False;User ID=your_user;Password=your_password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

		public async Task OnGetAsync()
		{
			// Fetch the logged-in user's ID or username (replace with actual logic)
			string loggedInUsername = User.Identity.Name ?? "JohnDoe"; // Replace with actual authentication logic

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
							User = new UserProfile
							{
								Username = reader.GetString(0),
								Email = reader.GetString(1),
								Role = reader.GetString(2),
								DateJoined = reader.GetDateTime(3)
							};
						}
						else
						{
							// Handle case where user is not found
							User = null;
						}
					}
				}
			}
		}

		public class UserProfile
		{
			public string Username { get; set; }
			public string Email { get; set; }
			public string Role { get; set; }
			public DateTime DateJoined { get; set; }
		}
	}
}
