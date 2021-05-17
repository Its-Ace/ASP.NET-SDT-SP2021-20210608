
using ServerAppClient;
using ServerAppClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerAppClient.Repo
{
	public interface IRepository<T> 
		where T:Entity 
	{
		void Add(T entity);

	}
}
