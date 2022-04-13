using Microsoft.AspNetCore.Mvc;

namespace Akot.PeanutButter.API.Web.Controllers
{
    class Record
    {
        public string Name { get; set; } = "Garfield";
        public int Age { get; set; } = 24;
    }
    [ApiController]
    [Route("api/[controller]")]
    public class RecordController : ControllerBase
    {
        public RecordController()
        {
        }

        [HttpGet()]
        public IActionResult Get()
        {
            //var data = Enumerable.Range(1, 1000).ToDictionary(v => v, v => Enumerable.Range(1, v).ToList());
            var data = new Record();
            return this.Ok(data);
        }
    }
}