using Microsoft.EntityFrameworkCore;
using ServerAppClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerAppClient.Context;

namespace ServerAppClient.Repo
{
	public class Repository<T> : IRepository<T> where T : Entity
	{
		private DbContext context;
		protected DbSet<T> objSet;

		public Repository(ItemContext context)
		{
			if(context == null)
			{
				System.Console.WriteLine("Context was null");
				throw new ArgumentNullException("context");
			}

			//If context was not null
			this.context = context;
			this.objSet = context.Set<T>();
		}
		public virtual void Add(T entity)
		{
			if (entity == null)
			{
				System.Console.WriteLine("No Entity was found to play with ");
				throw new ArgumentNullException("null : entity");
			}
			
			//Everything went fine
			objSet.Add(entity);
			context.SaveChanges();
		}

	
	}
}
