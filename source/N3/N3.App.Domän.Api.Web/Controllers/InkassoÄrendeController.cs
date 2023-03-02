using Microsoft.AspNetCore.Mvc;
using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.Kommando;
using N3.Modell;
using Rebus.Bus;

namespace N3.App.Domän.Api.Web.Controllers
{
    [ApiController]
    [ApiVersion("v1")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("v1/[controller]")]
    public class InkassoÄrendeController : ControllerBase
    {
        private readonly ILogger<InkassoÄrendeController> _logger;
        private readonly IBus _bus;

        public InkassoÄrendeController(ILogger<InkassoÄrendeController> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        [HttpGet]
        [Route("Import/Status/{aktivitetsIdentifierare}")]
        public async Task<IActionResult> HämtaStatusFörImportAvInkassoÄrende(
            [FromRoute] string aktivitetsIdentifierare
        )
        {
            await ValueTask.CompletedTask;
            using var logScope = _logger.BeginScope(aktivitetsIdentifierare);
            //var status = await _bus.HämtaStatus(aktivitetsIdentifierare);
            var status = new object();
            return Ok(status);
        }

        [HttpPost]
        [Route("Import")]
        public async Task<IActionResult> ImporteraInkassoÄrende(
            [FromBody] ImporteraInkassoÄrendeModell modell
        )
        {
            using var logScope = _logger.BeginScope(modell.AktivitetsIdentifierare);
            _logger.LogTrace("Lägger aktivitet för processering på aktivitetsbussen");
            ////await AktivitetsBuss.Kölägg(modell);
            ////await _bus.SendLocal(modell);
            await _bus.Send(modell);
            return Ok(modell.AktivitetsIdentifierare);
        }
    }
}
