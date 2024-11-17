using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MakebA_Inven_System.Pages
{
	public class InboxModel : PageModel
	{
		public List<Message> Messages { get; set; } = new List<Message>();

		private readonly string _connectionString = "Server=tcp:your_server.database.windows.net,1433;Initial Catalog=your_database;Persist Security Info=False;User ID=your_user;Password=your_password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

		public async Task OnGetAsync()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();

				string query = @"
                    SELECT Sender, Subject, Body, Timestamp 
                    FROM Messages 
                    WHERE Recipient = @Recipient";

				using (var command = new SqlCommand(query, connection))
				{
					// Replace "current_user" with the actual logged-in user's email or username
					command.Parameters.AddWithValue("@Recipient", "current_user@example.com");

					using (var reader = await command.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							Messages.Add(new Message
							{
								Sender = reader.GetString(0),
								Subject = reader.GetString(1),
								Body = reader.GetString(2),
								Timestamp = reader.GetDateTime(3)
							});
						}
					}
				}
			}
		}

		public class Message
		{
			public string Sender { get; set; }
			public string Subject { get; set; }
			public string Body { get; set; }
			public DateTime Timestamp { get; set; }
		}
	}
}
