using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace MakebA_Inven_System.Pages
{
    public class MessageDetailsModel : PageModel
    {
        private readonly string _connectionString = "Server=tcp:makebafinal.database.windows.net,1433;Initial Catalog=makeba;Persist Security Info=False;User ID=makeba;Password=Pls2s0727;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public EmailModel Message { get; set; }

        public void OnGet(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT EmailID, Sender, Recipient, Subject, Body, Timestamp
                        FROM Emails
                        WHERE EmailID = @EmailID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmailID", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Message = new EmailModel
                                {
                                    EmailID = reader.GetInt32(0),
                                    Sender = reader.GetString(1),
                                    Recipient = reader.GetString(2),
                                    Subject = reader.GetString(3),
                                    Body = reader.GetString(4),
                                    Timestamp = reader.GetDateTime(5)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public IActionResult OnPostDelete(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM Emails WHERE EmailID = @EmailID";

                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@EmailID", id);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            TempData["Message"] = "Email deleted successfully.";
                        }
                        else
                        {
                            TempData["Message"] = "Failed to delete email.";
                        }
                    }
                }

                return RedirectToPage("/Inbox");
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error deleting email: {ex.Message}";
                return RedirectToPage("/Inbox");
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
        }
    }
}
