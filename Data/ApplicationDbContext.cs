using Microsoft.EntityFrameworkCore;


namespace MakebA_Inven_System.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		
	}
}
