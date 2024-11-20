using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System;

namespace MakebA_Inven_System.Pages
{
    public class ReplyModel : PageModel
    {
        private readonly string _connectionString = "Server=tcp:makebafinal.database.windows.net,1433;Initial Catalog=makeba;Persist Security Info=False;User ID=makeba;Password=Pls2s0727;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [BindProperty]
        public ReplyInputModel Input { get; set; }

        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        public void OnGet(string replyTo, string subject)
        {
            // Populate the "To" and "Subject" fields
            To = replyTo;
            Subject = subject;
            Input = new ReplyInputModel
            {
                To = replyTo,
                Subject = subject
            };
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
                INSERT INTO Emails (Sender, Recipient, Subject, Body, Timestamp, Status)
                VALUES (@Sender, @Recipient, @Subject, @Body, GETDATE(), 'Sent')";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Retrieve the sender's email from the logged-in user's context
                        string senderEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

                        if (!string.IsNullOrEmpty(senderEmail))
                        {
                            command.Parameters.AddWithValue("@Sender", senderEmail);
                            command.Parameters.AddWithValue("@Recipient", Input.To);
                            command.Parameters.AddWithValue("@Subject", Input.Subject);
                            command.Parameters.AddWithValue("@Body", Input.Body);

                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }

                // Store the success message in TempData
                TempData["ReplySuccessMessage"] = "Reply sent successfully to all recipients!";
                ModelState.Clear(); // Clear the form
                return RedirectToPage(); // Stay on the same page
            }
            catch (Exception ex)
            {
                Message = $"An error occurred: {ex.Message}";
                return Page();
            }
        }



        public class ReplyInputModel
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
