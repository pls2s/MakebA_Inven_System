using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace MakebA_Inven_System.Pages
{
    public class ComposeModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

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
                        INSERT INTO Emails (Sender, Recipient, Subject, Body, Timestamp, Status)
                        VALUES (@Sender, @Recipient, @Subject, @Body, GETDATE(), 'Sent')";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Retrieve the logged-in user's email from claims
                        string loggedInSender = User.FindFirst(ClaimTypes.Email)?.Value;

                        if (string.IsNullOrEmpty(loggedInSender))
                        {
                            Message = "No logged-in user email found. Please log in.";
                            return Page();
                        }

                        command.Parameters.AddWithValue("@Sender", loggedInSender);
                        command.Parameters.AddWithValue("@Recipient", Input.To);
                        command.Parameters.AddWithValue("@Subject", Input.Subject);
                        command.Parameters.AddWithValue("@Body", Input.Body);

                        Console.WriteLine($"Inserting email: {loggedInSender} -> {Input.To}, Subject: {Input.Subject}");
                        await command.ExecuteNonQueryAsync();
                        Console.WriteLine("Message inserted successfully.");
                    }
                }
                Input = new InputModel();
                Message = "Message sent successfully!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Message = $"An error occurred: {ex.Message}";
            }

            return Page();
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Recipient email is required.")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
            public string To { get; set; }

            [Required(ErrorMessage = "Subject is required.")]
            public string Subject { get; set; }

            [Required(ErrorMessage = "Message body is required.")]
            public string Body { get; set; }
        }
    }
}
