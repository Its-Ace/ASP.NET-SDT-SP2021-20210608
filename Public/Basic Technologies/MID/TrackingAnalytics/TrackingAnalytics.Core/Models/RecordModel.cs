using System;
using System.Collections.Generic;
using System.Text;
using TrackingAnalytics;

namespace TrackingAnalytics.Core.Models
{
	public class RecordModel : Entity
	{
		public DateTime Time { get; set; }
		public string ip { get; set; }

		public string path { get; set; }

		public string query { get; set; }

		public string referer { get; set; }

		public string userAgent { get; set; }


	}
}
