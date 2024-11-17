using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace MakebA_Inven_System.Pages
{
	public class LoginModel : PageModel
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

					string query = @"
                        SELECT COUNT(*) FROM Users 
                        WHERE Username = @Username 
                        AND PasswordHash = @PasswordHash";

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@Username", Input.Username);
						command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

						int count = (int)await command.ExecuteScalarAsync();

						if (count > 0)
						{
							Message = "Login successful!";
							return RedirectToPage("/Index"); // เปลี่ยนเป็นหน้าเป้าหมายหลังล็อกอินสำเร็จ
						}
						else
						{
							Message = "Invalid username or password.";
							return Page();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Message = "An error occurred: " + ex.Message;
				return Page();
			}
		}

		private string HashPassword(string password)
		{
			using (var sha256 = SHA256.Create())
			{
				var bytes = Encoding.UTF8.GetBytes(password);
				var hash = sha256.ComputeHash(bytes);
				return Convert.ToBase64String(hash);
			}
		}

		public class InputModel
		{
			[Required(ErrorMessage = "Username is required.")]
			public string Username { get; set; } // เปลี่ยนเป็น Username อย่างเดียว

			[Required(ErrorMessage = "Password is required.")]
			public string Password { get; set; }
		}
	}
}
