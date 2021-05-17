using Lib.Net.Http.WebPush;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebPush;

namespace APIPushNotification.Models
{
	public class dataManager
	{
        public dataManager()
        {
            TimeStamp = DateTime.Now;
        }

		public dataManager(int id, string name, WebPush.PushSubscription subDetails, DateTime timeStamp)
		{
			Id = id;
			this.name = name;
			this.subDetails = subDetails;
			TimeStamp = timeStamp;
		}

		public int Id { get; set; }

        [Display(Name = "person name")]
        public string name { get; set; }

        [Display(Name = "Pushdescription")]
        public WebPush.PushSubscription subDetails { get; set; }

        public DateTime TimeStamp { get; set; }

     
    }
}

