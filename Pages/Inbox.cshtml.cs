using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace MakebA_Inven_System.Pages
{
	public class InboxModel : PageModel
	{
		public List<MessageModel> Messages { get; set; } = new List<MessageModel>();

		private readonly string _connectionString = "Server=tcp:makebafinal.database.windows.net,1433;Initial Catalog=makeba;Persist Security Info=False;User ID=makeba;Password=Pls2s0727;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

		public void OnGet()
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				string query = "SELECT Sender, Subject, Body, Timestamp FROM Emails WHERE Recipient = 'current_user@example.com'"; // เปลี่ยนเป็นอีเมลผู้ใช้งานจริง
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							Messages.Add(new MessageModel
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

		public class MessageModel
		{
			public string Sender { get; set; }
			public string Subject { get; set; }
			public string Body { get; set; }
			public DateTime Timestamp { get; set; }
		}
	}
}
