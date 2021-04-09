using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectAController : ControllerBase
    {

        private readonly IHttpClientFactory _httpClientFactory;
        public ProjectAController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/<ProjectAController>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var client = _httpClientFactory.CreateClient("MyNamedClient");
            var msg = await client.GetStringAsync("");

            return Ok(msg);
        }

        // GET api/<ProjectAController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProjectAController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ProjectAController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProjectAController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
