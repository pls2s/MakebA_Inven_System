using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;

namespace MakebA_Inven_System.Pages
{
	public class ComposeModel : PageModel
	{
		[BindProperty]
		public InputModel Input { get; set; }

		public string Message { get; set; } // Feedback message

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
						// Assuming you fetch the logged-in user's email as the sender
						string loggedInSender = "current_user@example.com"; // Replace with actual logged-in user
						command.Parameters.AddWithValue("@Sender", loggedInSender);
						command.Parameters.AddWithValue("@Recipient", Input.To);
						command.Parameters.AddWithValue("@Subject", Input.Subject);
						command.Parameters.AddWithValue("@Body", Input.Body);

						await command.ExecuteNonQueryAsync();
					}
				}

				Message = "Message sent successfully!";
				return RedirectToPage("/Inbox");
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
