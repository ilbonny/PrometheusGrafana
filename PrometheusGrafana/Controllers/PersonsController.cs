using Microsoft.AspNetCore.Mvc;
using PrometheusGrafana.MongoDb.Gateways;
using System.Threading.Tasks;
using PrometheusGrafana.Models;
using PrometheusGrafana.RabbitMq;
using PrometheusGrafana.RabbitMq.Models;
using System;
using Microsoft.AspNetCore.Http;

namespace PrometheusGrafana.Controllers
{
    [Route("api/[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonGateway _personGateway;
        private readonly IPublisherMessage<PersonAdded> _publisherAdded;
        private readonly IPublisherMessage<PersonModified> _publisherModified;
        private readonly IPublisherMessage<PersonDeleted> _publisherDeleted;

        public PersonsController(IPersonGateway personGateway, 
            IPublisherMessage<PersonAdded> publisherAdded,
            IPublisherMessage<PersonModified> publisherModified,
            IPublisherMessage<PersonDeleted> publisherDeleted)
        {
            _personGateway = personGateway;
            _publisherAdded = publisherAdded;
            _publisherModified = publisherModified;
            _publisherDeleted = publisherDeleted;
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

            _publisherAdded.Publish(new PersonAdded(entity.Id));

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut]
        public async Task<IActionResult> Save([FromBody] Person person)
        {
            person.Timestamp = DateTime.UtcNow;
            await _personGateway.Save(person);

            _publisherModified.Publish(new PersonModified(person.Id));
                
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _personGateway.Delete(id);
            _publisherDeleted.Publish(new PersonDeleted(id));

            return Ok();
        }
    }
}
