
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using ItemsService;
using System.Collections.Generic;
using System.Linq;
using ServerAppClient.Repo;
using ServerAppClient.Models;
using ServerAppClient.Context;

namespace ServerAppClient.Controllers
{
	[ApiController]
	[Route("api/items")]
	public class ItemController : Controller
	{
	
		public static ItemData model = null;
		private readonly ItemContext _context = null;
		private readonly IRepository<ItemData> _DataToDb = null;
		private readonly GrpcChannel channel;
		static int count = 0;
		public ItemController(IRepository<ItemData> dataToDb,ItemContext context)
		{
			_context = context;
			_DataToDb = dataToDb;
			channel = GrpcChannel.ForAddress("https://localhost:5001");
		}
		
		[HttpGet]
		public List<Item> GetAll()
		{
			
			var client = new ItemService.ItemServiceClient(channel);
			model = new ItemData();
			var data_returner = client.GetAll(new Empty()).Items.ToList();
			foreach (Item dummy in data_returner)
			{
				
				System.Console.WriteLine(_context.Item.FirstOrDefault(x => x.name == dummy.Name));
				while(count == 0)
				{
					model.name = dummy.Name;
					model.brand = dummy.Brand;
					model.seats = dummy.Seats;
					model.type = dummy.Type;
					model.value = dummy.Value;
					_context.Add(model);
					_context.SaveChanges();
					count++;
					System.Console.WriteLine("Data Refreshed");
				}


			}
			return client.GetAll(new Empty()).Items.ToList();
		}
		[HttpGet("{id}", Name = "GetProduct")]                                                   
		public IActionResult GetById(int id)
		{
			var client = new ItemService.ItemServiceClient(channel);
			var product = client.Get(new ItemId { Id = id });
			if (product == null)
			{
				return NotFound();
			}
			return new ObjectResult(product);
		}
		[HttpPost]
		public IActionResult Post([FromBody] Item product)
		{
			model = new ItemData();
			var client = new ItemService.ItemServiceClient(channel);
			var createdProduct = client.Insert(product);
			model.name = product.Name;
			model.brand = product.Brand;
			model.value = product.Value;
			model.seats = product.Seats;
			model.type = product.Type;
			_context.Add(model);
			_context.SaveChanges();
			return CreatedAtRoute("GetProduct", new { id = createdProduct.ItemId }, createdProduct);
		}
		[HttpPut]
		public IActionResult Put([FromBody] Item product)
		{
			var client = new ItemService.ItemServiceClient(channel);
			var udpatedProduct = client.Update(product);
			if (udpatedProduct == null)
			{
				return NotFound();
			}
			return NoContent();
		}
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var client = new ItemService.ItemServiceClient(channel);
			client.Delete(new ItemId { Id = id });
			return new ObjectResult(id);
		}

	}
}
