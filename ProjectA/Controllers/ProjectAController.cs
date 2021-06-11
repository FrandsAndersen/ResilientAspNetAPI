using Microsoft.AspNetCore.Mvc;
using Polly;
using ProjectA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectAController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRequestStringService _requestStringService;

        public ProjectAController(IHttpClientFactory httpClientFactory, IRequestStringService requestStringService)
        {
            _httpClientFactory = httpClientFactory;
            _requestStringService = requestStringService;
        }

        // GET: api/<ProjectAController>
        [HttpGet("NamedClient")]
        public async Task<IActionResult> GetAsyncNamedClient()
        {
            var client = _httpClientFactory.CreateClient("MyNamedClient");
            var response = await client.GetStringAsync("");
            return Ok(response);
        }

        // GET: api/<ProjectAController>
        [HttpGet("TypedClient")]
        public async Task<IActionResult> GetAsyncTypedClient()
        {
            //var response = await _requestStringService.GetStringsFromProjectB();
            var response = await _requestStringService.GetStringsFromProjectB();
            return Ok(response);
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
