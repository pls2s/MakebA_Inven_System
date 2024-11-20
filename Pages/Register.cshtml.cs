using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace MakebA_Inven_System.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public string Message { get; set; } // ข้อความแสดงผล (สำเร็จหรือข้อผิดพลาด)

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

            string hashedPassword = HashPassword(Input.Password);

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Check for duplicate email or username
                    string checkQuery = @"
                SELECT COUNT(*) FROM Users 
                WHERE Email = @Email OR Username = @Username";

                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Email", Input.Email);
                        checkCommand.Parameters.AddWithValue("@Username", Input.Username);

                        int count = (int)await checkCommand.ExecuteScalarAsync();

                        if (count > 0)
                        {
                            Message = "Email or Username already exists.";
                            return Page();
                        }
                    }

                    // Insert new user
                    string query = @"
                INSERT INTO Users (FirstName, LastName, Username, Email, Phone, Department, PasswordHash)
                VALUES (@FirstName, @LastName, @Username, @Email, @Phone, @Department, @PasswordHash)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", Input.FirstName);
                        command.Parameters.AddWithValue("@LastName", Input.LastName);
                        command.Parameters.AddWithValue("@Username", Input.Username);
                        command.Parameters.AddWithValue("@Email", Input.Email);
                        command.Parameters.AddWithValue("@Phone", Input.Phone); // Save phone number
                        command.Parameters.AddWithValue("@Department", Input.Department);
                        command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                Message = "Registration successful!";
                TempData["ShowPopup"] = true;
                ModelState.Clear();
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Message = "An error occurred: " + ex.Message;
                return Page();
            }
        }


        // ฟังก์ชันสำหรับแฮชรหัสผ่าน
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Input model สำหรับการกรอกข้อมูล
        public class InputModel
        {
            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [RegularExpression(@"^\d+$", ErrorMessage = "Phone number can only contain numbers.")]
            [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 digits.")]
            public string Phone { get; set; } // New field for phone number

            [Required]
            public string Department { get; set; }

            [Required]
            public string Password { get; set; }

            [Required]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
        }

    }
}

