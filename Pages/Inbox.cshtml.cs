using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace MakebA_Inven_System.Pages
{
    public class InboxModel : PageModel
    {
        private readonly string _connectionString = "Server=tcp:makebafinal.database.windows.net,1433;Initial Catalog=makeba;Persist Security Info=False;User ID=makeba;Password=Pls2s0727;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public List<EmailModel> Messages { get; set; } = new List<EmailModel>();
        public List<EmailModel> FilteredMessages { get; set; } = new List<EmailModel>();

        public string LoggedInUserEmail { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterOption { get; set; }

        public void OnGet()
        {
            try
            {
                // Retrieve email from claims added during login
                LoggedInUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(LoggedInUserEmail))
                {
                    Console.WriteLine("No logged-in user email found.");
                    return;
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT EmailID, Sender, Recipient, Subject, Body, Timestamp, Status
                        FROM Emails
                        WHERE Recipient = @Recipient
                        ORDER BY Timestamp DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Recipient", LoggedInUserEmail);

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
                                    Timestamp = reader.GetDateTime(5),
                                    Status = reader.GetString(6)
                                };
                                Messages.Add(email);
                            }
                        }
                    }
                }

                // Apply search query and filter
                ApplyFilters();
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void ApplyFilters()
        {
            FilteredMessages = Messages;

            if (!string.IsNullOrEmpty(SearchQuery))
            {
                FilteredMessages = FilteredMessages.Where(email =>
                    (FilterOption == "Subject" && email.Subject.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (FilterOption == "Date" && email.Timestamp.ToString("dd MMM yyyy").Contains(SearchQuery)) ||
                    (string.IsNullOrEmpty(FilterOption) && // Default to search all fields
                        (email.Sender.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                         email.Subject.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                         email.Timestamp.ToString("dd MMM yyyy").Contains(SearchQuery)))
                ).ToList();
            }
        }

        public class EmailModel
        {
            public int EmailID { get; set; }
            public string Sender { get; set; }
            public string Recipient { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public DateTime Timestamp { get; set; }
            public string Status { get; set; } // Keep this if Status is still relevant
        }
    }
}
