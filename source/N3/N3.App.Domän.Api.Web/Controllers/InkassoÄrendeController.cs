using Microsoft.AspNetCore.Mvc;
using N3.App.Domän.Api.Web.Messages;
using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.JobbPaket;
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
        private readonly IJobbKö _jobbKö;

        public InkassoÄrendeController(
            ILogger<InkassoÄrendeController> logger,
            IBus bus,
            IJobbKö jobbKö
        )
        {
            _logger = logger;
            _bus = bus;
            _jobbKö = jobbKö;
        }

        [HttpGet]
        [Route("Import/Status/Alla")]
        [ProducesResponseType(200, Type = typeof(ImporteraInkassoÄrendeModell[]))]
        public async Task<IActionResult> HämtaStatusFörImportAvInkassoÄrenden()
        {
            using var logScope = _logger.BeginScope("Import/Status/Alla");
            var data = await _jobbKö.HämtaStatus<ImporteraInkassoÄrendeJobb>();
            return Ok(data);
        }

        [HttpGet]
        [Route("Import/Status/{aktivitetsIdentifierare}")]
        [ProducesResponseType(200, Type = typeof(ImporteraInkassoÄrendeModell))]
        [ProducesResponseType(404, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> HämtaStatusFörImportAvInkassoÄrende(
            [FromRoute] string aktivitetsIdentifierare
        )
        {
            using var logScope = _logger.BeginScope(aktivitetsIdentifierare);
            var data = await _jobbKö.HämtaStatus<ImporteraInkassoÄrendeJobb>(
                aktivitetsIdentifierare
            );
            return Ok(data);
        }

        public record KöläggningsKvitto(string Id);

        public record KonfliktId(string Id);

        public record ImporteraInkassoÄrendeModell(ImporteraInkassoÄrendeJobb Jobb);

        [HttpPost]
        [Route("Import/Kölägg")]
        [ProducesResponseType(200, Type = typeof(KöläggningsKvitto))]
        [ProducesResponseType(400, Type = typeof(ProblemDetails))]
        [ProducesResponseType(409, Type = typeof(KonfliktId))]
        public async Task<IActionResult> ImporteraInkassoÄrende(
            [FromBody] ImporteraInkassoÄrendeModell modell
        )
        {
            using var logScope = _logger.BeginScope(modell);
            _logger.LogTrace(
                "Lägger aktivitet för processering på aktivitetsbussen (id={id})",
                modell.Jobb.Id
            );
            await _bus.SendLocal(new ImporteraInkassoÄrendeJobbKommando(modell.Jobb));
            return Ok(new KöläggningsKvitto(modell.Jobb.Id));
        }
    }
}
