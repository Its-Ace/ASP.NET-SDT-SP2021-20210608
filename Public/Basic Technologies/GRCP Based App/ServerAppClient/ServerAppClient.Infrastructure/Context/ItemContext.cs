using Microsoft.EntityFrameworkCore;
using ServerAppClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ServerAppClient.Context
{
	public class ItemContext : DbContext
	{
		public DbSet<ItemData> Item { get; set; }
		//Contexing the class which we created in the context
		public ItemContext(DbContextOptions<ItemContext> options) : base(options)
		{

		}
	}
}
