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

        public async Task<IActionResult> OnPostAsync(bool sendAnyway = false)
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

                    // Check if the recipient email exists in the database
                    if (!sendAnyway)
                    {
                        string checkRecipientQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Recipient";
                        using (SqlCommand checkCommand = new SqlCommand(checkRecipientQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@Recipient", Input.To);
                            int recipientCount = (int)await checkCommand.ExecuteScalarAsync();

                            // If recipient email does not exist
                            if (recipientCount == 0)
                            {
                                TempData["RecipientNotFound"] = true; // Flag to show the warning
                                TempData["RecipientEmail"] = Input.To; // Save the email for display
                                TempData["Subject"] = Input.Subject; // Save the subject
                                TempData["Body"] = Input.Body; // Save the body
                                return RedirectToPage(); // Reload the page with the warning
                            }
                        }
                    }

                    // Insert the email into the database
                    string query = @"
            INSERT INTO Emails (Sender, Recipient, Subject, Body, Timestamp, Status)
            VALUES (@Sender, @Recipient, @Subject, @Body, GETDATE(), 'Sent')";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        string senderEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
                        command.Parameters.AddWithValue("@Sender", senderEmail);
                        command.Parameters.AddWithValue("@Recipient", Input.To);
                        command.Parameters.AddWithValue("@Subject", Input.Subject);
                        command.Parameters.AddWithValue("@Body", Input.Body);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                Message = "Message sent successfully!";
                ModelState.Clear(); // Clear form inputs
                Input = new InputModel(); // Reset the form
                return Page(); // Stay on the same page
            }
            catch (Exception ex)
            {
                Message = $"An error occurred: {ex.Message}";
                return Page();
            }
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
