using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerAppClient.Models
{
	public class ItemData : Entity
	{
		public string name { get; set; }

		public int seats { get; set; }

		public string brand { get; set; }

		public float value { get; set; }

		public string type { get; set; }

	}
}
