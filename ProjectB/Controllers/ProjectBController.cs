using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Polly;
using System.Threading.Tasks;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectBController : ControllerBase
    {
        // GET: api/<ProjectBController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var isEnabled = true;

            var fault = new SocketException(errorCode: 10013);
            var chaosPolicy = MonkeyPolicy.InjectException(with =>
                with.Fault(fault)
                                                        .InjectionRate(0.5) // Fail 50% of requests
                                                                            //.InjectionRate(1) // Used to simulate circuit breaker
                .Enabled(isEnabled));

            var result = chaosPolicy.Execute(() => GetSomeString());

            return result;
        }

        private string[] GetSomeString()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ProjectBController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProjectBController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ProjectBController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProjectBController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
