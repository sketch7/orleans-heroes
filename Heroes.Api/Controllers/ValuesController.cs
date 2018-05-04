using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Heroes.Api.Controllers
{
	[Route("api/[controller]")]
	public class ValuesController : Controller
	{
		private readonly IConfiguration _config;

		public ValuesController(IConfiguration config)
		{
			_config = config;
		}
		// GET api/values
		[HttpGet]
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2", _config.GetValue<string>("yo") };
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/values
		[HttpPost]
		public void Post([FromBody]string value)
		{
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}