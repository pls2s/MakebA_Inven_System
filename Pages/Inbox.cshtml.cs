using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace MakebA_Inven_System.Pages
{
	public class InboxModel : PageModel
	{
		public List<Message> Messages { get; set; }

		public void OnGet()
		{
			// Dummy data simulating messages from a database
			Messages = new List<Message>
			{
				new Message { Id = 1, Sender = "John Doe", Subject = "New Inventory Alert", Timestamp = DateTime.Now, IsNew = true, IsRead = false },
				new Message { Id = 2, Sender = "System Admin", Subject = "Weekly Report Available", Timestamp = DateTime.Now.AddHours(-5), IsNew = false, IsRead = true },
				new Message { Id = 3, Sender = "Jane Smith", Subject = "Profile Update Required", Timestamp = DateTime.Now.AddDays(-1), IsNew = false, IsRead = false }
			};
		}

		public class Message
		{
			public int Id { get; set; }
			public string Sender { get; set; }
			public string Subject { get; set; }
			public DateTime Timestamp { get; set; }
			public bool IsNew { get; set; }
			public bool IsRead { get; set; }
		}
	}
}
