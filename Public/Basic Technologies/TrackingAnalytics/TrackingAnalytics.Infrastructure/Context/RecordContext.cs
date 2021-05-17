using Microsoft.EntityFrameworkCore;
using TrackingAnalytics.Core.Models;

namespace TrackingAnalytics.Infrastructure.Context
{
	public class RecordContext : DbContext
	{
		public DbSet<RecordModel> Records { get; set; }
		public RecordContext(DbContextOptions<RecordContext> options) : base(options)
		{

		}
		
	}
}
