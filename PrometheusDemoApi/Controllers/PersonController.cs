using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PrometheusDemoApi.Models;

namespace PrometheusDemoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<Person>> GetAll()
        {
            return Ok(new List<Person>());
        }

        [HttpGet("{id}")]
        public ActionResult<Person> Get(string id)
        {
            return Ok(new Person("1", "Marco", "Bonny"));
        }
    }
}
