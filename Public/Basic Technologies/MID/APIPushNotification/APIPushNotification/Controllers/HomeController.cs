using APIPushNotification.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebPush;

namespace APIPushNotification.Controllers
{
	public class HomeController : Controller
	{
		public int members = 0;
		public string activeUser;
		static List<dataManager> _newPersons = null;
		private readonly ILogger<HomeController> _logger;
		private readonly IConfiguration configuration;
		public void flushData()
		{
			_newPersons.Add(new dataManager
			{
				Id = 1,
				subDetails = null,
				name = "Ali",
				TimeStamp = DateTime.UtcNow

			}
			);
		}



		public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
		{
			this.configuration = configuration;
			_logger = logger;
			//flushData();
			if (_newPersons == null)
			{
				ViewBag.PreviousCount = 0;
			}
			else
			{
				ViewBag.PreviousCount = Convert.ToInt64(_newPersons.Count());
			}
		}

		//[ok

		public IActionResult Index()
		{
			//Used by our Service worker js
			ViewBag.applicationServerKey = configuration["VAPID:publicKey"];
			System.Diagnostics.Debug.WriteLine("View one is used");
			return View();

		}

		[HttpPost]
		public IActionResult Index(string client, string endpoint, string p256dh, string auth)
		{
			System.Diagnostics.Debug.WriteLine("View Two is used");
			HttpContext.Session.SetString("active_user", client);
			TempData["client_name"] = client;
			var previous_count = (_newPersons == null) ? 0 : _newPersons.Count();
			
			if (client == null)
			{
				return BadRequest("No Client Name parsed.");
			}
		
			if(_newPersons != null)
			{
				if (_newPersons.Any(z => z.name == client))
				{
					return BadRequest("Client Name already used.");
				}
				var subscription = new PushSubscription(endpoint, p256dh, auth);
				_newPersons.Add(new dataManager
				{
					Id = members,
					name = client,
					TimeStamp = DateTime.Now,
					subDetails = subscription
				});
				members++;
				System.Diagnostics.Debug.WriteLine("Second Step");
			}
			else
			{
				//We do not need to match any if there is no existance
				var subscription = new PushSubscription(endpoint, p256dh, auth);
				_newPersons = new List<dataManager>();
				_newPersons.Add(new dataManager
				{
					Id = members,
					name = client,
					TimeStamp = DateTime.Now,
					subDetails = subscription
				});
				members++;
				ViewBag.PreviousCount = Convert.ToInt64(previous_count);
				System.Diagnostics.Debug.WriteLine("First Step");
				return View(_newPersons.ToList());
				
			}

			ViewBag.PreviousCount = Convert.ToInt64(previous_count);
			return View(_newPersons.ToList());
		}

		public IActionResult Notify()
		{
			ViewBag.Active_user = Convert.ToInt32(members);
			if (_newPersons == null)
			{
				return View();
			}
			else
			{

				ViewBag.Active_user = HttpContext.Session.GetString("active_user");
				
				return View(_newPersons.ToList());
			}
			
		}

		[HttpPost]
		public IActionResult Notify(string message, string client)
		{
			
			if (client == null)
			{
				return BadRequest("No Client Name parsed.");
			}

			PushSubscription subscription = _newPersons.FirstOrDefault(x => x.name == client).subDetails;
			System.Diagnostics.Debug.WriteLine(subscription);
			if (subscription == null)
			{
				return BadRequest("Client was not found");
			}

			var subject = configuration["VAPID:subject"];
			var publicKey = configuration["VAPID:publicKey"];
			var privateKey = configuration["VAPID:privateKey"];

			var vapidDetails = new VapidDetails(subject, publicKey, privateKey);

			var webPushClient = new WebPushClient();
			try
			{
				webPushClient.SendNotification(subscription, message, vapidDetails);
			}
			catch (Exception exception)
			{
				// Log error

				//If sql insertion show exception
				System.Diagnostics.Debug.WriteLine(exception.Message);

			}

			return View(_newPersons.ToList());
		}
		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
