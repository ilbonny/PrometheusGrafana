using Microsoft.AspNetCore.Mvc;
using PrometheusGrafana.Gateways;
using System.Threading.Tasks;

namespace PrometheusGrafana.Controllers
{
    [Route("api/[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonGateway _personGateway;

        public PersonsController(IPersonGateway personGateway)
        {
            _personGateway = personGateway;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var person = await _personGateway.Get(id);
            return person.Match(
                    Some: p => Ok(p),
                    None: (IActionResult)NotFound());
        }
    }
}
