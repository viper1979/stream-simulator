using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StreamSimulator.Core;

namespace StreamSimulatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        private readonly StreamManager _manager;

        public StreamController(StreamManager manager)
        {
            _manager = manager;
        }

        // GET api/stream
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return _manager.GetSimulators();
        }

        // GET api/stream/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var settings = new StreamSettings
            {
                ListeningPort = 9999,
                AllowSubscribe = true,
                HeartbeatIntervalMs = 5000,
                SendReplyCommands = true,
            };
            _manager.AddSimulator(settings);

            return "Ok";
        }

        // POST api/stream
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/stream/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/stream/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
