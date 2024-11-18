using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace MakebA_Inven_System.Pages
{
    public class SentModel : PageModel
    {
        private readonly string _connectionString = "Server=tcp:makebafinal.database.windows.net,1433;Initial Catalog=makeba;Persist Security Info=False;User ID=makeba;Password=Pls2s0727;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public List<Emails> SentMessages { get; set; } = new List<Emails>();

        public string LoggedInUserEmail { get; set; }

        public void OnGet()
        {
            // Fetch the logged-in user's email from the claims
            LoggedInUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(LoggedInUserEmail))
            {
                Console.WriteLine("No logged-in user email found.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT EmailID, Sender, Recipient, Subject, Body, Timestamp, Status
                        FROM Emails
                        WHERE Sender = @Sender
                        ORDER BY Timestamp DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Sender", LoggedInUserEmail);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SentMessages.Add(new Emails
                                {
                                    EmailID = reader.GetInt32(0),
                                    Sender = reader.GetString(1),
                                    Recipient = reader.GetString(2),
                                    Subject = reader.GetString(3),
                                    Body = reader.GetString(4),
                                    Timestamp = reader.GetDateTime(5),
                                    Status = reader.GetString(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    public class Emails
    {
        public int EmailID { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
    }
}
