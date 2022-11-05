using Microsoft.AspNetCore.Mvc;
using Nereid.Services;
using System.Net;

namespace Nereid.Controllers
{
    [ApiController]
    public class ListController : Controller
    {
        public ListController(DataStore dataStore)
        {
            DataStore = dataStore;
        }

        private DataStore DataStore { get; }

        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [HttpPost]
        [Route("List")]
        public IActionResult AddList()
        {
            using var sr = new StreamReader(Request.Body);
            var name = sr.ReadToEnd();
            if (DataStore.AddList(name))
            {
                return NoContent();
            }

            return Conflict();
        }

        public record ListGetArguments(int? Skip, int? Take);

        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet]
        [Route("List/{name}")]
        public IActionResult GetList(string name, [FromQuery]ListGetArguments? args)
        {
            var list = DataStore.GetList(name);
            if (list is null)
            {
                return NotFound();
            }

            IEnumerable<byte[]> result = list;
            if (args.Skip is int skip)
            {
                result = result.Skip(skip);
            }
            if (args.Take is int take)
            {
                result = result.Take(take);
            }

            result = result.ToList();

            return Ok(result);
        }

        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpPost]
        [Route("List/{name}/Append")]
        public IActionResult ListAppend(string name)
        {
            using var ms = new MemoryStream();
            Request.Body.CopyTo(ms);
            var data = ms.ToArray();
            if( DataStore.AppendToList(name, data))
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
