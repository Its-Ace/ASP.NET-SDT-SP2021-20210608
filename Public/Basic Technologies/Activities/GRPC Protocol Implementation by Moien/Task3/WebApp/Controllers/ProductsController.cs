using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using ProductsService;
using System.Collections.Generic;
using System.Linq;
namespace SimpleTalkGrpcWebClient.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController : Controller
	{
		private readonly GrpcChannel channel;
		public ProductsController()
		{
			channel = GrpcChannel.ForAddress("https://localhost:5001");
		}
		[HttpGet]
		public List<Product> GetAll()
		{
			var client = new ProductService.ProductServiceClient(channel);
			return client.GetAll(new Empty()).Products.ToList();
		}
		[HttpGet("{id}", Name = "GetProduct")]
		public IActionResult GetById(int id)
		{
			var client = new ProductService.ProductServiceClient(channel);
			var product = client.Get(new ProductId { Id = id });
			if (product == null)
			{
				return NotFound();
			}
			return new ObjectResult(product);
		}
		[HttpPost]
		public IActionResult Post([FromBody] Product product)
		{
			var client = new ProductService.ProductServiceClient(channel);
			var createdProduct = client.Insert(product);

			return CreatedAtRoute("GetProduct", new { id = createdProduct.ProductId }, createdProduct);
		}
		[HttpPut]
		public IActionResult Put([FromBody] Product product)
		{
			var client = new ProductService.ProductServiceClient(channel);
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
			var client = new ProductService.ProductServiceClient(channel);
			client.Delete(new ProductId { Id = id });
			return new ObjectResult(id);
		}
	}
}