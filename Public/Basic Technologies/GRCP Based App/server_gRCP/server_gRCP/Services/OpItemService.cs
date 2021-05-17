
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
namespace ItemsService
{
	public class OpItemService : ItemService.ItemServiceBase
	{
		private readonly List<Item> _items = new List<Item>();
		private int idCount = 0;
		private readonly ILogger<OpItemService> _logger;
		public OpItemService(ILogger<OpItemService> logger)
		{
			_logger = logger;

			_items.Add(new Item()
				{
					ItemId = idCount++,
					Name = "Car Dummy",
					Seats = 4,
					Type = "LTV",
					Brand = "Mehran",
					Value = 20000000

				}
			);
		}
		public override Task <ItemsList> GetAll(Empty empty, ServerCallContext context)
		{
			ItemsList pl = new ItemsList();
			pl.Items.AddRange(_items);
			return Task.FromResult(pl);
		}
		public override Task<Item> Get(ItemId itemId, ServerCallContext context)
		{
			return Task.FromResult( //
				(from p in _items where p.ItemId == itemId.Id select p).FirstOrDefault());
		}
		public override Task<Item> Insert(Item item, ServerCallContext context)
		{
			item.ItemId = idCount++;
			_items.Add(item);
			return Task.FromResult(item);
		}
		public override Task<Item> Update(Item item, ServerCallContext context)
		{
			var productToUpdate = (from p in _items where p.ItemId == item.ItemId select p).FirstOrDefault();
			if (productToUpdate != null)
			{
				productToUpdate.Name = item.Name;
				productToUpdate.Seats = item.Seats;
				productToUpdate.Brand = item.Brand;
				productToUpdate.Type = item.Type;
				productToUpdate.Value = item.Value;
				return Task.FromResult(item);
			}
			return Task.FromException<Item>(new EntryPointNotFoundException());
		}
		public override Task<Empty> Delete(ItemId itemId, ServerCallContext context)
		{
			var productToDelete = (from p in _items where p.ItemId == itemId.Id select p).FirstOrDefault();
			if (productToDelete == null)
			{
				return Task.FromException<Empty>(new EntryPointNotFoundException());
			}
			_items.Remove(productToDelete);
			return Task.FromResult(new Empty());
		}
	}
}
