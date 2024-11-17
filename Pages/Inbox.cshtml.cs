using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace MakebA_Inven_System.Pages
{
	public class InboxModel : PageModel
	{
		private readonly string _connectionString = "Server=tcp:makebafinal.database.windows.net,1433;Initial Catalog=makeba;Persist Security Info=False;User ID=makeba;Password=Pls2s0727;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

		public List<EmailModel> Messages { get; set; } = new List<EmailModel>();

		public string LoggedInUser { get; set; } // Holds the logged-in user's email

		public void OnGet()
		{
			// Use logged-in user's email, replace this with dynamic authentication logic
			LoggedInUser = User.Identity?.Name ?? "current_user@example.com";

			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					string query = @"
                        SELECT EmailID, Sender, Recipient, Subject, Body, IsRead, IsNew, Timestamp, Status
                        FROM Emails
                        WHERE Recipient = @Recipient
                        ORDER BY Timestamp DESC";

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@Recipient", LoggedInUser);

						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								var email = new EmailModel
								{
									EmailID = reader.GetInt32(0),
									Sender = reader.GetString(1),
									Recipient = reader.GetString(2),
									Subject = reader.GetString(3),
									Body = reader.GetString(4),
									IsRead = reader.GetBoolean(5),
									IsNew = reader.GetBoolean(6),
									Timestamp = reader.GetDateTime(7),
									Status = reader.GetString(8)
								};
								Messages.Add(email);
							}
						}
					}
				}
			}
			catch (SqlException sqlEx)
			{
				Console.WriteLine($"SQL Error: {sqlEx.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
			}
		}

		public class EmailModel
		{
			public int EmailID { get; set; }
			public string Sender { get; set; }
			public string Recipient { get; set; }
			public string Subject { get; set; }
			public string Body { get; set; }
			public bool IsRead { get; set; }
			public bool IsNew { get; set; }
			public DateTime Timestamp { get; set; }
			public string Status { get; set; }
		}
	}
}
