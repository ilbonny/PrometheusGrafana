using Microsoft.AspNetCore.Mvc;
using PrometheusGrafana.MongoDb.Gateways;
using System.Threading.Tasks;
using PrometheusGrafana.Models;
using PrometheusGrafana.RabbitMq;
using System;
using Microsoft.AspNetCore.Http;

namespace PrometheusGrafana.Controllers
{
    [Route("api/[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonGateway _personGateway;
        private readonly IPersonPublisher _personPublisher;

        public PersonsController(IPersonGateway personGateway, IPersonPublisher personPublisher)
        {
            _personGateway = personGateway;
            _personPublisher = personPublisher;
        }

        [HttpGet("{id}", Name = "GetById")]
        public async Task<IActionResult> GetById(string id)
        {
            var person = await _personGateway.Get(id);
            return person.Match(
                    Some: p => Ok(p),
                    None: (IActionResult)NotFound());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Insert([FromBody] Person person)
        {
            if (person is null)
                return BadRequest(new ArgumentNullException());

            person.Timestamp = DateTime.UtcNow;
            var entity = await _personGateway.Insert(person);
            _personPublisher.Publish(entity);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut]
        public async Task<IActionResult> Save([FromBody] Person person)
        {
            person.Timestamp = DateTime.UtcNow;
            await _personGateway.Save(person);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _personGateway.Delete(id);
            return Ok();
        }
    }
}
