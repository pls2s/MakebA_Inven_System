using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace MakebA_Inven_System.Pages
{
	public class ComposeModel : PageModel
	{
		[BindProperty]
		public InputModel Input { get; set; }

		public string Message { get; set; }

		private readonly string _connectionString = "Server=tcp:makebafinal.database.windows.net,1433;Initial Catalog=makeba;Persist Security Info=False;User ID=makeba;Password=Pls2s0727;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

		public void OnGet() { }

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string query = @"
                        INSERT INTO Emails (Sender, Recipient, Subject, Body, IsRead, IsNew, Timestamp, Status)
                        VALUES (@Sender, @Recipient, @Subject, @Body, 0, 1, GETDATE(), 'Sent')";

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						string loggedInSender = User.Identity?.Name ?? "default_user@example.com";
						command.Parameters.AddWithValue("@Sender", loggedInSender);
						command.Parameters.AddWithValue("@Recipient", Input.To);
						command.Parameters.AddWithValue("@Subject", Input.Subject);
						command.Parameters.AddWithValue("@Body", Input.Body);

						await command.ExecuteNonQueryAsync();
					}
				}

				Message = "Message sent successfully!";
				return RedirectToPage("/Inbox"); // กลับไปยังหน้า Inbox หลังส่งข้อความ
			}
			catch (Exception ex)
			{
				Message = $"An error occurred: {ex.Message}";
				return Page();
			}
		}

		public class InputModel
		{
			[Required]
			[EmailAddress]
			public string To { get; set; }

			[Required]
			public string Subject { get; set; }

			[Required]
			public string Body { get; set; }
		}
	}
}
